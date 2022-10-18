namespace Cherry
{
    internal class RoutingTable
    {
        private readonly Dictionary<string, HttpController> _routingTable = new();

        internal void Add(
            string path,
            HttpController controller)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            _routingTable.Add(path, controller);
        }

        internal bool TryGetControllerForRouteStartingWith(string path, out HttpController controller)
        {
            controller = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            foreach (var entry in _routingTable)
            {
                var route = entry.Key;

                if (route.StartsWith(path))
                {
                    controller = entry.Value;
                    break;
                }
            }

            return controller != null;
        }

        internal bool HasRouteStartingWith(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            foreach (var entry in _routingTable)
            {
                var route = entry.Key;

                if (route.StartsWith(path))
                    return true;
            }

            return false;
        }
    }
}
