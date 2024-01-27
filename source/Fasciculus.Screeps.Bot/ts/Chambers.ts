import { Territories, Territory } from "./Territories";
import { Dictionary, GameWrap, Vector } from "./Common";

export class Chamber
{
    private _terrain?: RoomTerrain;
    private _sources: Vector<Source>

    readonly room: Room;

    get name(): string { return this.room.name; }

    get energyCapacity(): number { return this.room.energyCapacityAvailable; }

    get terrain(): RoomTerrain { return this._terrain || (this._terrain = this.room.getTerrain()); }
    get territory(): Territory { return Territories.get(this.room); }

    get sources(): Vector<Source> { return this._sources.clone(); }

    constructor(room: Room)
    {
        this.room = room;
        this._sources = Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
    }
}

export class Chambers
{
    private static _all: Vector<Chamber> = new Vector();
    private static _byName: Dictionary<Chamber> = {};

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._byName[name] : undefined; }

    static get all(): Vector<Chamber> { return Chambers._all.clone(); }

    static initialize()
    {
        Chambers._all = GameWrap.rooms.map(r => new Chamber(r));
        Chambers._byName = Chambers._all.indexBy(c => c.name);
    }
}