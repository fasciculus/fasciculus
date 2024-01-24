
import { Vector, Vectors } from "./Collections";

export class GameWrap
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    static myCreep(name: string): Creep | undefined { return Game.creeps[name]; }
    static get myCreeps(): Vector<Creep> { return Vectors.values(Game.creeps); }

    static get myFlags(): Vector<Flag> { return Vectors.values(Game.flags); }
    static get myPowerCreeps(): Vector<PowerCreep> { return Vectors.values(Game.powerCreeps); }
    static get rooms(): Vector<Room> { return Vectors.values(Game.rooms); }
    static get mySpawns(): Vector<StructureSpawn> { return Vectors.values(Game.spawns); }
    static get myStructures(): Vector<Structure> { return Vectors.values(Game.structures); }
    static get myConstructionSites(): Vector<ConstructionSite> { return Vectors.values(Game.constructionSites); }
}