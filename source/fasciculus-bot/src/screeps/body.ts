
export class Bodies
{
    private static _infos: Map<CreepId, BodyInfo> = new Map();

    private static createInfo(id: CreepId, hint?: Creep): BodyInfo
    {
        const creep: Creep | undefined = hint || Game.get(id);

        if (!creep) return { work: 0 };

        var work: number = 0;

        creep.body.forEach(d =>
        {
            switch (d.type)
            {
                case WORK: ++work; break;
            }
        });

        return { work };
    }

    static workParts(creep: Creep)
    {
        return Bodies._infos.ensure(creep.id, Bodies.createInfo, creep).work;
    }

    static cleanup()
    {
        Bodies._infos.keep(Game.existing(Bodies._infos.ids));
    }
}