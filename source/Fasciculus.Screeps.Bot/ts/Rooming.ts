import { Dictionary, GameWrap, Point, Vector } from "./Common";

export type FieldType = 0 | TERRAIN_MASK_WALL | TERRAIN_MASK_SWAMP;

export class Territory
{
    readonly terrain: RoomTerrain;

    constructor(terrain: RoomTerrain)
    {
        this.terrain = terrain;
    }

    at(p: Point): FieldType { return this.terrain.get(p.x, p.y); }
    around(p: Point): Vector<FieldType> { return p.around().map(q => this.at(q)); }
}

export class Territories
{
    private static _cache: Dictionary<Territory> = {};

    static get(room: Room): Territory
    {
        let name = room.name;
        let result: Territory | undefined = Territories._cache[name];

        if (!result)
        {
            result = Territories._cache[name] = new Territory(room.getTerrain());
        }

        return result;
    }
}

export class Chamber
{
    readonly room: Room;
    readonly name: string;
    readonly controller?: StructureController;
    readonly energyCapacityAvailable: number;

    readonly my: boolean;
    readonly controlled: boolean;

    private readonly _sources: Vector<Source>

    get territory(): Territory { return Territories.get(this.room); }

    get sources(): Vector<Source> { return this._sources.clone(); }

    constructor(room: Room)
    {
        this.room = room;
        this.name = room.name;
        this._sources = Vector.from(room.find<FIND_SOURCES, Source>(FIND_SOURCES));
        this.energyCapacityAvailable = room.energyCapacityAvailable;

        const controller = room.controller;

        this.controller = controller;

        if (controller)
        {
            const reservation = controller?.reservation;

            this.my = controller.my;
            this.controlled = this.my || reservation?.username == GameWrap.username;
        }
        else
        {
            this.my = false;
            this.controlled = false;
        }
    }
}

export class Chambers
{
    private static _all: Vector<Chamber> = new Vector();
    private static _my: Vector<Chamber> = new Vector();
    private static _controlled: Vector<Chamber> = new Vector();
    private static _byName: Dictionary<Chamber> = {};

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._byName[name] : undefined; }

    static get all(): Vector<Chamber> { return Chambers._all.clone(); }
    static get my(): Vector<Chamber> { return Chambers._my.clone(); }
    static get controlled(): Vector<Chamber> { return Chambers._controlled.clone(); }

    static initialize()
    {
        Chambers._all = GameWrap.rooms.map(r => new Chamber(r));
        Chambers._my = Chambers._all.filter(c => c.my);
        Chambers._controlled = Chambers._all.filter(c => c.controlled);
        Chambers._byName = Chambers._all.indexBy(c => c.name);
    }
}
