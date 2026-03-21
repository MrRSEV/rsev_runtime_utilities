# namespace: rsev_utilities.di.inject

def inject(container, **dependencies):
    def decorator(cls):
        for attr, key in dependencies.items():
            setattr(cls, attr, container.resolve(key))
        return cls
    return decorator