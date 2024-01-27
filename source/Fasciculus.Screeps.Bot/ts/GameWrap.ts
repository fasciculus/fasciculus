
import { Dictionaries } from "./Collections";
import { Vector } from "./Common";

export class GameWrap
{
    static get<T extends _HasId>(id: Id<T> | undefined): T | undefined
    {
        let result: T | null = id ? Game.getObjectById(id) : null;

        return result || undefined;
    }

    static myCreep(name: string): Creep | undefined { return Game.creeps[name]; }
    static get myCreeps(): Vector<Creep> { return Dictionaries.values(Game.creeps); }

    static get myFlags(): Vector<Flag> { return Dictionaries.values(Game.flags); }
    static get myPowerCreeps(): Vector<PowerCreep> { return Dictionaries.values(Game.powerCreeps); }
    static get rooms(): Vector<Room> { return Dictionaries.values(Game.rooms); }
    static get mySpawns(): Vector<StructureSpawn> { return Dictionaries.values(Game.spawns); }
    static get myStructures(): Vector<Structure> { return Dictionaries.values(Game.structures); }
    static get myConstructionSites(): Vector<ConstructionSite> { return Dictionaries.values(Game.constructionSites); }
}