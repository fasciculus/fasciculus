import { Vector, Vectors } from "./Collections";
import { CreepType } from "./Enums";

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

export interface BodyPartCounts
{
    work: number;
}

export class BodyParts
{
    static comparePriority(a: BodyPartConstant, b: BodyPartConstant)
    {
        let pa: number = PartPriorities[a] || 99;
        let pb: number = PartPriorities[b] || 99;

        return pa - pb;
    }

    static costOf(parts: Vector<BodyPartConstant>): number
    {
        return parts.sum(p => BODYPART_COST[p]);
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
        return BodyParts.countsOf(creep).work;
    }
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
        let chunk: BodyChunk | undefined = BodyTemplate.createChunk(Vector.from(parts));

        if (!chunk) return this;

        for (let i = 0; i < times; ++i)
        {
            this.chunks.append(chunk);
        }

        return this;
    }

    createBody(energy: number): Vector<BodyPartConstant> | undefined
    {
        let chunkCount = this.chunkCount(energy);

        if (chunkCount == 0) return undefined;

        return Vectors.flatten(this.chunks.take(chunkCount).map(c => c.parts)).sort(BodyParts.comparePriority);
    }

    private chunkCount(energy: number): number
    {
        var result: number = 0;

        for (let chunk of this.chunks)
        {
            if (energy < chunk.cost) break;

            ++result;
        }

        return result;
    }

    private static createChunk(parts: Vector<BodyPartConstant>): BodyChunk | undefined
    {
        if (parts.length == 0) return undefined;

        let result: BodyChunk =
        {
            cost: BodyParts.costOf(parts),
            parts: parts
        };

        return result;
    }
}

export class Bodies
{
    private static _registry: { [type: string]: BodyTemplate } = {}

    static template(type: CreepType): BodyTemplate | undefined { return Bodies._registry[type]; }

    static register(type: CreepType, template: BodyTemplate)
    {
        Bodies._registry[type] = template;
    }

    static createBody(type: CreepType, energy: number): Vector<BodyPartConstant> | undefined
    {
        return Bodies.template(type)?.createBody(energy);
    }
}