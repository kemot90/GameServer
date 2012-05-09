using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dijkstra
{
    public class Connection
    {
        uint _a, _b;
        int _weight;

        public Connection(uint a, uint b, int weight)
        {
            this._a = a;
            this._b = b;
            this._weight = weight;
        }

        public uint A
        {
            get { return _a; }
            set { _a = value; }
        }

        public uint B
        {
            get { return _b; }
            set { _b = value; }
        }

        public int Time
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }


    public class Route
    {
        int _cost;
        List<Connection> _connections;
        uint _identifier;

        public Route(uint _identifier)
        {
            _cost = int.MaxValue;
            _connections = new List<Connection>();
            this._identifier = _identifier;
        }

        public List<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        public int Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }
    }


    public class Dijkstra
    {
        List<Connection> _connections;
        List<uint> _locations;

        public List<uint> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }

        public List<Connection> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        public Dijkstra()
        {
            _connections = new List<Connection>();
            _locations = new List<uint>();
        }

        /// <summary>
        /// Calculates the shortest route to all the other locations
        /// </summary>
        /// <param name="_startLocation"></param>
        /// <returns>List of all locations and their shortest route</returns>
        public int CalculateMinCost(uint _startLocation, uint _stopLocation)
        {
            //Initialise a new empty route list
            Dictionary<uint, Route> _shortestPaths = new Dictionary<uint, Route>();
            //Initialise a new empty handled locations list
            List<uint> _handledLocations = new List<uint>();
            
            //Initialise the new routes. the constructor will set the route weight to in.max
            foreach (uint location in _locations)
            {
                _shortestPaths.Add(location, new Route(location));
            }

            //The startPosition has a weight 0
            _shortestPaths[_startLocation].Cost = 0;
            

            //If all locations are handled, stop the engine and return the result
            while (_handledLocations.Count != _locations.Count)
            {
                //Order the locations
                List<uint> _shortestLocations = (List < uint > )(from s in _shortestPaths
                                                        orderby s.Value.Cost                                       
                                                        select s.Key).ToList();
                
                uint _locationToProcess = 0;

                //Search for the nearest location that isn't handled
                foreach (uint _location in _shortestLocations)
                {
                    if (!_handledLocations.Contains(_location))
                    {
                        //If the cost equals int.max, there are no more possible connections to the remaining locations
                        if (_shortestPaths[_location].Cost == int.MaxValue)
                            return _shortestPaths[_stopLocation].Cost;
                        _locationToProcess = _location;
                        break;
                    }
                }

                //Select all connections where the startposition is the location to Process
                var _selectedConnections = from c in _connections
                                           where c.A == _locationToProcess
                                           select c;

                //Iterate through all connections and search for a connection which is shorter
                foreach (Connection conn in _selectedConnections)
                {
                    if (_shortestPaths[conn.B].Cost > conn.Time + _shortestPaths[conn.A].Cost)
                    {
                        _shortestPaths[conn.B].Connections = _shortestPaths[conn.A].Connections.ToList();
                        _shortestPaths[conn.B].Connections.Add(conn);
                        _shortestPaths[conn.B].Cost = conn.Time + _shortestPaths[conn.A].Cost;
                    }
                }
                //Add the location to the list of processed locations
                _handledLocations.Add(_locationToProcess);
            }

            return _shortestPaths[_stopLocation].Cost;
        }
    }
}
