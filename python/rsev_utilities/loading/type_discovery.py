import inspect


class TypeDiscovery:

    @staticmethod
    def find_classes(module, predicate=None):
        classes = [member for _, member in inspect.getmembers(module, inspect.isclass)]
        if predicate is None:
            return classes
        return [member for member in classes if predicate(member)]
