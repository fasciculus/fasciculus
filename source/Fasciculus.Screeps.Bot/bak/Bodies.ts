
import * as _ from "lodash";

const MIN_WORKER_BODY: BodyPartConstant[] = [WORK, CARRY, MOVE, MOVE];

export const MIN_WORKER_COST = _.sum(MIN_WORKER_BODY.map(b => BODYPART_COST[b]));
const MAX_WORKER_COST = MIN_WORKER_COST * 12;

export class Bodies
{
    static workerEnergy(capacity: number): number
    {
        let n = Math.floor(capacity / MIN_WORKER_COST);

        return Math.max(MIN_WORKER_COST, Math.min(MAX_WORKER_COST, n * MIN_WORKER_COST));
    }

    static workerBody(energy: number): BodyPartConstant[] | undefined
    {
        let n = Math.floor(energy / MIN_WORKER_COST);

        if (n < 1) return undefined;

        let works: BodyPartConstant[] = _.range(n).map(i => WORK);
        let carries: BodyPartConstant[] = _.range(n).map(i => CARRY);
        let moves: BodyPartConstant[] = _.range(n * 2).map(i => MOVE);

        return works.concat(carries).concat(moves);
    }
}