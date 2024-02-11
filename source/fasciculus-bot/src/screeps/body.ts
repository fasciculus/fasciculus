
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

    private static _templates: Map<string, BodyTemplate> = new Map();

    private _chunks: Array<BodyChunk> = new Array();

    constructor(type: string)
    {
        BodyTemplate._templates.set(type, this);
    }

    add(times: number, ...parts: Array<BodyPartConstant>): BodyTemplate
    {
        const cost = parts.sum(p => BODYPART_COST[p]);
        const chunk: BodyChunk = { cost, parts };

        for (var i = 0; i < times; ++i)
        {
            this._chunks.push(chunk);
        }

        return this;
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

    static createTemplate(type: string, times: number, ...parts: Array<BodyPartConstant>): BodyTemplate
    {
        return new BodyTemplate(type).add(times, ...parts);
    }

    static get(type: string): BodyTemplate | undefined
    {
        return BodyTemplate._templates.get(type);
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