from __future__ import annotations

import asyncio
from threading import RLock


class RuntimeProcessController:
    """Thread-safe registry and lifecycle controller for managed processes."""

    def __init__(self, logger=None):
        self.logger = logger
        self._processes: dict[str, object] = {}
        self._lock = RLock()

    def register(self, process) -> bool:
        if process is None:
            raise ValueError("process must not be None")
        with self._lock:
            if process.process_id in self._processes:
                return False
            self._processes[process.process_id] = process
            return True

    def register_range(self, processes) -> int:
        return sum(1 for process in processes if self.register(process))

    def exists(self, process_id: str) -> bool:
        with self._lock:
            return process_id in self._processes

    def get(self, process_id: str):
        with self._lock:
            return self._processes.get(process_id)

    def get_all(self) -> list:
        with self._lock:
            return list(self._processes.values())

    def find_by_name(self, name: str) -> list:
        with self._lock:
            return [p for p in self._processes.values() if p.name.casefold() == name.casefold()]

    def find_by_state(self, state) -> list:
        with self._lock:
            return [p for p in self._processes.values() if p.state == state]

    def unregister(self, process_id: str) -> bool:
        with self._lock:
            return self._processes.pop(process_id, None) is not None

    async def start_async(self, process_id: str) -> bool:
        process = self.get(process_id)
        return bool(process and await process.start_async())

    async def start_by_name_async(self, name: str) -> bool:
        processes = self.find_by_name(name)
        return bool(processes and await processes[0].start_async())

    async def stop_async(self, process_id: str) -> bool:
        process = self.get(process_id)
        return bool(process and await process.stop_async())

    async def stop_by_name_async(self, name: str) -> bool:
        processes = self.find_by_name(name)
        return bool(processes and await processes[0].stop_async())

    async def start_all_async(self) -> int:
        results = await asyncio.gather(*(p.start_async() for p in self.get_all()))
        return sum(bool(result) for result in results)

    async def stop_all_async(self) -> int:
        results = await asyncio.gather(*(p.stop_async() for p in self.get_all()))
        return sum(bool(result) for result in results)

    async def check_all_health_async(self) -> dict[str, bool | None]:
        processes = self.get_all()
        results = await asyncio.gather(*(p.check_health_async() for p in processes), return_exceptions=True)
        return {
            process.process_id: (False if isinstance(result, Exception) else result)
            for process, result in zip(processes, results)
        }

    def list_processes(self) -> list[tuple[str, str, object]]:
        return [(p.process_id, p.name, p.state) for p in self.get_all()]
