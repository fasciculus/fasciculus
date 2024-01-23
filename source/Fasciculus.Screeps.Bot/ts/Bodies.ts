import * as _ from "lodash";
import { Vector, Vectors } from "./Collections";

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
    parts: Vector<BodyPartConstant>;
}

export class BodyTemplate
{
    private chunks: Vector<BodyChunk> = new Vector();

    static create(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        return new BodyTemplate().add(parts, times);
    }

    add(parts: BodyPartConstant[], times: number = 1): BodyTemplate
    {
        let chunk: BodyChunk | undefined = BodyTemplate.createChunk(Vectors.from(parts));

        if (!chunk) return this;

        for (let i = 0; i < times; ++i)
        {
            this.chunks.append(chunk);
        }

        return this;
    }

    get(energy: number): Vector<BodyPartConstant>
    {
        var result: Vector<BodyPartConstant> = new Vector();

        for (let chunk of this.chunks)
        {
            if (energy < chunk.cost) break;

            result = result.concat(chunk.parts);
            energy -= chunk.cost;
        }

        return result;
    }

    static costOf(parts: Vector<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
    }

    private static createChunk(parts: Vector<BodyPartConstant>): BodyChunk | undefined
    {
        if (parts.length == 0) return undefined;

        let result: BodyChunk =
        {
            cost: BodyTemplate.costOf(parts),
            parts: parts
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
    private static _registry: { [type: string]: BodyTemplate } = {}

    static register(type: string, template: BodyTemplate)
    {
        Bodies._registry[type] = template;
    }

    static create(type: string, energy: number): Vector<BodyPartConstant>
    {
        var template = Bodies._registry[type];

        if (!template) return new Vector();

        return template.get(energy).sort(Bodies.compareParts);
    }

    private static compareParts(a: BodyPartConstant, b: BodyPartConstant): number
    {
        return PartPriorities[a] - PartPriorities[b];
    }

    static countsOf(creep: Creep): BodyPartCounts
    {
        let result: BodyPartCounts = { work: 0 };

        for (let part of creep.body)
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
        return Bodies.countsOf(creep).work;
    }
}