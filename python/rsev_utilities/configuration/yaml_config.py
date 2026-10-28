# namespace: rsev_utilities.configuration.yaml_config

import json
from pathlib import Path


class YamlConfig:

    @staticmethod
    def load(path: str):
        lines = [line for line in Path(path).read_text(encoding="utf-8").splitlines() if line.strip() and not line.lstrip().startswith("#")]
        if not lines:
            return {}
        # JSON is a valid YAML subset and gives callers a complete stdlib path.
        try:
            return json.loads("\n".join(lines))
        except json.JSONDecodeError:
            return _parse_yaml(lines)

    @staticmethod
    def save(path: str, data):
        Path(path).write_text(_dump_mapping(data), encoding="utf-8")


def _scalar(value: str):
    value = value.strip()
    if value in {"true", "True"}:
        return True
    if value in {"false", "False"}:
        return False
    if value in {"null", "Null", "~"}:
        return None
    try:
        return json.loads(value)
    except json.JSONDecodeError:
        return value.strip("'\"")


def _parse_yaml(lines):
    tokens = [(len(line) - len(line.lstrip()), line.strip()) for line in lines]

    def block(index, indentation):
        is_list = tokens[index][1].startswith("-")
        result = [] if is_list else {}
        while index < len(tokens) and tokens[index][0] == indentation:
            text = tokens[index][1]
            if is_list:
                if not text.startswith("-"):
                    break
                value = text[1:].strip()
                if value:
                    result.append(_scalar(value))
                    index += 1
                elif index + 1 < len(tokens) and tokens[index + 1][0] > indentation:
                    child, index = block(index + 1, tokens[index + 1][0])
                    result.append(child)
                else:
                    result.append(None)
                    index += 1
            else:
                if ":" not in text:
                    index += 1
                    continue
                key, value = text.split(":", 1)
                value = value.strip()
                if value:
                    result[key.strip()] = _scalar(value)
                    index += 1
                elif index + 1 < len(tokens) and tokens[index + 1][0] > indentation:
                    child, index = block(index + 1, tokens[index + 1][0])
                    result[key.strip()] = child
                else:
                    result[key.strip()] = None
                    index += 1
        return result, index

    return block(0, tokens[0][0])[0]


def _dump_mapping(data, indent=0):
    output = []
    for key, value in data.items():
        if isinstance(value, dict):
            output.append(" " * indent + f"{key}:")
            output.append(_dump_mapping(value, indent + 2))
        elif isinstance(value, list):
            output.append(" " * indent + f"{key}:")
            output.extend(" " * (indent + 2) + f"- {json.dumps(item)}" for item in value)
        else:
            output.append(" " * indent + f"{key}: {json.dumps(value)}")
    return "\n".join(output)
