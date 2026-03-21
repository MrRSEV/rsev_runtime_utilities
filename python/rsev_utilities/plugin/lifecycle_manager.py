# namespace: rsev_utilities.plugin.lifecycle_manager


class LifecycleManager:

    @staticmethod
    def enable(plugin):
        plugin.on_enable()

    @staticmethod
    def disable(plugin):
        plugin.on_disable()
