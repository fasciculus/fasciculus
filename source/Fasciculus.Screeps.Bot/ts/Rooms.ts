import { profile } from "./Profiling";
import { Cached } from "./screeps.util";

export class Chamber
{
    readonly name: string;

    get room(): Room | undefined { return Room.get(this.name); }
    get controller(): StructureController | undefined { return this.room?.controller; }

    constructor(name: string)
    {
        this.name = name;
    }
}

export class Chambers
{
    private static _allChambers: Cached<Map<string, Chamber>> = Cached.withValue(Chambers.fetchAllChambers);
    private static _allControllers: Cached<Set<ControllerId>> = Cached.simple(Chambers.fetchAllControllers);

    static get all(): Array<Chamber> { return Chambers._allChambers.value.vs(); }
    static get allControllers(): Set<ControllerId> { return Chambers._allControllers.value.clone(); }

    @profile
    private static fetchAllChambers(value: Map<string, Chamber> | undefined): Map<string, Chamber>
    {
        const result: Map<string, Chamber> = value || new Map();

        if (result.update(Room.names, name => new Chamber(name)))
        {
            Chambers._allControllers.reset();
        }

        return result;
    }

    private static fetchAllControllers(): Set<ControllerId>
    {
        return Array.defined(Chambers.all.map(c => c.controller)).map(c => c.id).toSet();
    }
}
