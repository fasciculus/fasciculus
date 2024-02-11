
type BodyChunk = { cost: number; parts: Array<BodyPartConstant>; };

export class BodyTemplate
{
    private static _priorities: {[part: string]: number} =
    {
        "tough": 1,
        "work": 2,
        "attack": 3,
        "ranged_attack": 4,
        "carry": 5,
        "move": 6,
        "heal": 7,
        "claim": 8
    };

    private _chunks: Array<BodyChunk> = new Array();

    add(times: number, ...parts: Array<BodyPartConstant>): BodyTemplate
    {
        const cost = BodyTemplate.cost(...parts);
        const chunk: BodyChunk = { cost, parts };

        for (var i = 0; i < times; ++i)
        {
            this._chunks.push(chunk);
        }

        return this;
    }

    static createTemplate(times: number, ...parts: Array<BodyPartConstant>): BodyTemplate
    {
        return new BodyTemplate().add(times, ...parts);
    }

    static cost(...parts: Array<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
    }

    createBody(energy: number): Array<BodyPartConstant> | undefined
    {
        const chunkCount: number = this.chunkCount(energy);

        if (chunkCount == 0) return undefined;

        return Array.flatten(this._chunks.take(chunkCount).map(c => c.parts)).sort(BodyTemplate.compare)
    }

    private chunkCount(energy: number): number
    {
        var result: number = 0;

        for (let chunk of this._chunks)
        {
            if (energy < chunk.cost) break;

            ++result;
            energy -= chunk.cost;
        }

        return result;
    }

    private static compare(a: BodyPartConstant, b: BodyPartConstant)
    {
        let pa: number = BodyTemplate._priorities[a] || 99;
        let pb: number = BodyTemplate._priorities[b] || 99;

        return pa - pb;
    }
}

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