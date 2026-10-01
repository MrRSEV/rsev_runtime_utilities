from __future__ import annotations

import asyncio
import os
import signal
import subprocess
import sys
import threading
import uuid
from abc import ABC, abstractmethod
from enum import Enum
from typing import Callable


class SubprocessState(str, Enum):
    CREATED = "created"
    STARTING = "starting"
    RUNNING = "running"
    STOPPING = "stopping"
    STOPPED = "stopped"
    FAULTED = "faulted"


class ProcessLogLevel(str, Enum):
    INFO = "info"
    WARNING = "warning"
    ERROR = "error"


class IRuntimeSubprocess(ABC):
    @property
    @abstractmethod
    def process_id(self) -> str:
        raise NotImplementedError

    @property
    @abstractmethod
    def name(self) -> str:
        raise NotImplementedError

    @property
    @abstractmethod
    def executable(self) -> str:
        raise NotImplementedError

    @property
    @abstractmethod
    def arguments(self) -> list[str]:
        raise NotImplementedError

    @property
    @abstractmethod
    def state(self) -> SubprocessState:
        raise NotImplementedError

    async def start_async(self) -> bool:
        raise NotImplementedError

    async def stop_async(self) -> bool:
        raise NotImplementedError

    async def restart_async(self) -> bool:
        await self.stop_async()
        return await self.start_async()

    async def check_health_async(self) -> bool | None:
        return self.state == SubprocessState.RUNNING


class RuntimeSubprocessBase(IRuntimeSubprocess):
    """Managed subprocess with lifecycle state and stdout/stderr callbacks."""

    def __init__(self, name: str, *, logger=None):
        self._process_id = uuid.uuid4().hex[:12]
        self._name = name
        self._state = SubprocessState.CREATED
        self._exit_code: int | None = None
        self.properties: dict[str, object] = {}
        self.logger = logger
        self.on_stdout: list[Callable] = []
        self.on_stderr: list[Callable] = []
        self._process: subprocess.Popen | None = None
        self._lock = threading.RLock()

    @property
    def process_id(self) -> str:
        return self._process_id

    @property
    def name(self) -> str:
        return self._name

    @property
    def state(self) -> SubprocessState:
        with self._lock:
            return self._state

    @property
    def exit_code(self) -> int | None:
        return self._exit_code

    @property
    @abstractmethod
    def executable(self) -> str:
        raise NotImplementedError

    @property
    @abstractmethod
    def arguments(self) -> list[str]:
        raise NotImplementedError

    @property
    def command(self) -> list[str]:
        return [self.executable, *self.arguments]

    def add_stdout_handler(self, callback: Callable) -> None:
        if callback not in self.on_stdout:
            self.on_stdout.append(callback)

    def add_stderr_handler(self, callback: Callable) -> None:
        if callback not in self.on_stderr:
            self.on_stderr.append(callback)

    async def start_async(self) -> bool:
        return await asyncio.to_thread(self.start)

    def start(self) -> bool:
        with self._lock:
            if self._state == SubprocessState.RUNNING:
                return True
            self._state = SubprocessState.STARTING
            self._exit_code = None

        try:
            process = subprocess.Popen(
                self.command,
                stdin=subprocess.DEVNULL,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True,
                bufsize=1,
                creationflags=getattr(subprocess, "CREATE_NEW_PROCESS_GROUP", 0),
                start_new_session=os.name != "nt",
            )
            with self._lock:
                self._process = process
                self._state = SubprocessState.RUNNING
            threading.Thread(target=self._pump, args=(process.stdout, self.on_stdout), daemon=True).start()
            threading.Thread(target=self._pump, args=(process.stderr, self.on_stderr), daemon=True).start()
            threading.Thread(target=self._watch_exit, args=(process,), daemon=True).start()
            return True
        except Exception as error:
            self._state = SubprocessState.FAULTED
            self._log("error", f"[{self._name}] failed to start: {error}")
            return False

    async def stop_async(self) -> bool:
        return await asyncio.to_thread(self.stop)

    def stop(self) -> bool:
        with self._lock:
            process = self._process
            if process is None or process.poll() is not None:
                self._state = SubprocessState.STOPPED
                return True
            self._state = SubprocessState.STOPPING

        try:
            if os.name == "nt":
                process.terminate()
            else:
                os.killpg(os.getpgid(process.pid), signal.SIGTERM)
        except (ProcessLookupError, OSError):
            process.terminate()

        try:
            process.wait(timeout=5)
        except subprocess.TimeoutExpired:
            process.kill()
            process.wait(timeout=5)
        with self._lock:
            self._exit_code = process.returncode
            self._state = SubprocessState.STOPPED
        return True

    async def restart_async(self) -> bool:
        await self.stop_async()
        return await self.start_async()

    async def check_health_async(self) -> bool | None:
        return self.state == SubprocessState.RUNNING and self._process is not None and self._process.poll() is None

    def _pump(self, stream, callbacks: list[Callable]) -> None:
        if stream is None:
            return
        for line in stream:
            value = line.rstrip("\r\n")
            for callback in tuple(callbacks):
                try:
                    callback(self, value)
                except TypeError:
                    callback(value)
                except Exception:
                    continue

    def _watch_exit(self, process: subprocess.Popen) -> None:
        code = process.wait()
        with self._lock:
            self._exit_code = code
            if self._state != SubprocessState.STOPPING:
                self._state = SubprocessState.STOPPED if code == 0 else SubprocessState.FAULTED
        if code != 0:
            self._log("warning", f"[{self._name}] exited with code {code}")

    def _log(self, level: str, message: str) -> None:
        if self.logger is None:
            return
        method = getattr(self.logger, level, None)
        if callable(method):
            method(message)


class ExecutableProcess(RuntimeSubprocessBase):
    def __init__(self, name: str, executable: str, arguments: list[str] | None = None, *, logger=None):
        super().__init__(name, logger=logger)
        self._executable = executable
        self._arguments = list(arguments or [])

    @property
    def executable(self) -> str:
        return self._executable

    @property
    def arguments(self) -> list[str]:
        return list(self._arguments)


class ShellProcess(ExecutableProcess):
    def __init__(self, name: str, command: str, *, logger=None):
        shell = os.environ.get("COMSPEC", "cmd.exe") if os.name == "nt" else "/bin/sh"
        args = ["/c", command] if os.name == "nt" else ["-c", command]
        super().__init__(name, shell, args, logger=logger)


class PythonProcess(ExecutableProcess):
    def __init__(self, name: str, script: str, arguments: list[str] | None = None, *, logger=None):
        super().__init__(name, sys.executable, [script, *(arguments or [])], logger=logger)


class NodeProcess(ExecutableProcess):
    def __init__(self, name: str, script: str, arguments: list[str] | None = None, *, logger=None):
        super().__init__(name, "node", [script, *(arguments or [])], logger=logger)


class DotNetProcess(ExecutableProcess):
    def __init__(self, name: str, assembly: str, arguments: list[str] | None = None, *, logger=None):
        super().__init__(name, "dotnet", [assembly, *(arguments or [])], logger=logger)
