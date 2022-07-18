using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data
{
    public class SceneModel
    {
        public List<Satellite> Satellites { get; set; }
        public List<GroundTarget> GroundTargets { get; set; }
        public List<Footprint> Footprints { get; set; }
        public List<GroundStation> GroundStations { get; set; }

        protected SceneModel()
        {

        }

        public static SceneModel Build()
        {
            var sceneModel = new SceneModel()
            {
                Satellites = new List<Satellite>(),
                GroundTargets = new List<GroundTarget>(),
                Footprints = new List<Footprint>(),
                GroundStations = new List<GroundStation>(),               
            };

            var satellites = SatelliteBuilder.Create();
            var footprints = FootprintBuilder.Create(satellites);
            var targets = GroundTargetBuilder.Create(footprints.ToList());
            var gs = GroundStationBuilder.Create();

            sceneModel.Satellites.AddRange(satellites);
            sceneModel.Footprints.AddRange(footprints);
            sceneModel.GroundTargets.AddRange(targets);
            sceneModel.GroundStations.AddRange(gs);

            return sceneModel;
        }
    }
}
