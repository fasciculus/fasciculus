const WORKER_BODIES =
    [
        [WORK, CARRY, MOVE]
    ];

function getBodyCost(parts: BodyPartConstant[]): number
{
    var result = 0;

    for (var part of parts)
    {
        result += BODYPART_COST[part];
    }

    return result;
}

const WORKER_COSTS = WORKER_BODIES.map(body => getBodyCost(body))

export class BodyManager
{
    static createWorkerBody(room: Room): BodyPartConstant[]
    {
        const energyAvailable = room.energyAvailable;

        for (let i = WORKER_COSTS.length - 1; i >= 0; --i)
        {
            let cost = WORKER_COSTS[i];

            if (cost <= energyAvailable)
            {
                return WORKER_BODIES[i];
            }
        }

        return [];
    }
}