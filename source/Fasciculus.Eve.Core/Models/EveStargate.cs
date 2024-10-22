using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveStargate : EveObject
    {
        private readonly EveId destinationId;

        public EveStargate(EveId id, EveId destinationId)
            : base(id)
        {
            this.destinationId = destinationId;
        }

        public override void Write(Data data)
        {
            base.Write(data);

            destinationId.Write(data);
        }

        public static EveStargate Read(Data data)
        {
            EveId id = EveId.Read(data);
            EveId destinationId = EveId.Read(data);

            return new EveStargate(id, destinationId);
        }
    }
}
