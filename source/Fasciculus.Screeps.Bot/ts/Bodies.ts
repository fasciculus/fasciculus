import * as _ from "lodash";

export interface BodyTemplate
{
    minSize: number;

    parts: BodyPartConstant[];
}

export class Bodies
{
    static _registry: { [type: string]: BodyTemplate } = {}

    static register(type: string, minSize: number, parts: BodyPartConstant[])
    {
        Bodies._registry[type] = { minSize, parts };
    }

    static create(type: string, energy: number): BodyPartConstant[] | undefined
    {
        var template = Bodies._registry[type];

        if (!template) return undefined;

        var result = _.take(template.parts, template.minSize);
        var minCost = _.sum(result.map(p => BODYPART_COST[p]));

        if (energy < minCost) return undefined;

        energy -= minCost;

        for (let i = template.minSize, n = template.parts.length; i < n; ++i)
        {
            var part = template.parts[i];
            var cost = BODYPART_COST[part];

            if (energy < cost) break;

            result.push(part);
            energy -= cost;
        }

        return result;
    }
}