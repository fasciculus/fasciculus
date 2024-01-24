
import * as _ from "lodash";
import { Vector, Vectors } from "./Collections";

export class GameWrap
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    static creep(name: string): Creep | undefined { return Game.creeps[name]; }
    static get myCreeps(): Vector<Creep> { return Vectors.values(Game.creeps); }

    static get myFlags(): Flag[] { return _.values(Game.flags); }
    static get myPowerCreeps(): PowerCreep[] { return _.values(Game.powerCreeps); }
    static get rooms(): Vector<Room> { return Vector.from(_.values(Game.rooms)); }
    static get mySpawns(): StructureSpawn[] { return _.values(Game.spawns); }
    static get myStructures(): Structure[] { return _.values(Game.structures); }
    static get myConstructionSites(): ConstructionSite[] { return _.values(Game.constructionSites); }
}