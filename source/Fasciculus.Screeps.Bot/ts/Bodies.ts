import * as _ from "lodash";

const PartPriorities =
{
    "tough": 1,
    "work": 2,
    "attack": 3,
    "ranged_attack": 4,
    "carry": 5,
    "move": 6,
    "heal": 7,
    "claim": 8
}

export interface BodyChunk
{
    cost: number;
    parts: BodyPartConstant[];
}

export class BodyTemplate
{
    private chunks: BodyChunk[] = [];

    static create(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        return new BodyTemplate().add(parts, times);
    }

    add(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        let chunk: BodyChunk = BodyTemplate.createChunk(parts);

        for (let i = 0; i < times; ++i)
        {
            this.chunks.push(chunk);
        }

        return this;
    }

    get(energy: number): BodyPartConstant[]
    {
        var result: BodyPartConstant[] = [];

        for (let chunk of this.chunks)
        {
            if (energy < chunk.cost) break;

            result = result.concat(chunk.parts);
            energy -= chunk.cost;
        }

        return result;
    }

    static costOf(parts: BodyPartConstant[]): number
    {
        return parts.length == 0 ? 0 : _.sum(parts.map(p => BODYPART_COST[p]));
    }

    private static createChunk(parts: BodyPartConstant[]): BodyChunk
    {
        let result: BodyChunk =
        {
            cost: BodyTemplate.costOf(parts),
            parts: Array.from(parts)
        };

        return result;
    }
}

export interface BodyPartCounts
{
    work: number;
}

export class Bodies
{
    static _registry: { [type: string]: BodyTemplate } = {}

    static register(type: string, template: BodyTemplate)
    {
        Bodies._registry[type] = template;
    }

    static create(type: string, energy: number): BodyPartConstant[] | undefined
    {
        var template = Bodies._registry[type];

        if (!template) return undefined;

        var result: BodyPartConstant[] = template.get(energy);

        if (result.length == 0) return undefined;

        return result.sort(Bodies.compareParts);
    }

    private static compareParts(a: BodyPartConstant, b: BodyPartConstant): number
    {
        return PartPriorities[a] - PartPriorities[b];
    }

    static countsOf(parts: BodyPartDefinition[]): BodyPartCounts
    {
        let result: BodyPartCounts = { work: 0 };

        for (let part of parts)
        {
            switch (part.type)
            {
                case WORK: ++result.work; break;
            }
        }

        return result;
    }

    static workOf(creep: Creep): number
    {
        return Bodies.countsOf(creep.body).work;
    }
}