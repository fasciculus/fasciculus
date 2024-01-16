
import * as _ from "lodash";

export enum CreepType
{
    Weller = "W",
    Supplier = "S"
}

export interface ICreepMemory
{
    type: CreepType
}

export class Creeps
{
    private static _all: Creep[] = [];
    private static _my: Creep[] = [];
    private static _ofType: _.Dictionary<Creep[]> = {};

    static get all(): Creep[] { return Creeps._all; }
    static get my(): Creep[] { return Creeps._my; }

    static ofType(type: CreepType): Creep[] { return Creeps._ofType[type] || []; }

    static typeOf(creep: Creep): CreepType
    {
        let memory = creep.memory as ICreepMemory;

        return memory.type;
    }

    static initialize()
    {
        Creeps._all = _.values(Game.creeps);
        Creeps._my = Creeps._all.filter(c => c.my);
        Creeps._ofType = _.groupBy(Creeps._my, Creeps.typeOf);
    }
}