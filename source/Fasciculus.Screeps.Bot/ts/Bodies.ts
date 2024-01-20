import * as _ from "lodash";

export interface BodyChunk
{
    cost: number;
    parts: BodyPartConstant[];
}

export interface BodyTemplate
{
    chunks: BodyChunk[];
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

        var result: BodyPartConstant[] = [];

        for (let chunk of template.chunks)
        {
            if (energy < chunk.cost) break;

            result = result.concat(chunk.parts);
            energy -= chunk.cost;
        }

        return result.length > 0 ? result : undefined;
    }
}