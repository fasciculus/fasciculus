using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveStargate : EveObject
    {
        private readonly int destinationId;

        public EveStargate(int id, int destinationId)
            : base(id)
        {
            this.destinationId = destinationId;
        }

        public override void Write(Data data)
        {
            base.Write(data);

            data.WriteInt(destinationId);
        }
    }
}
