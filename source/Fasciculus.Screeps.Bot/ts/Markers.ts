import { Dictionaries, Dictionary, GameWrap, MarkerType, Vector } from "./Common";
import { Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Wellers } from "./Workers";

const INFO_MARKER_TEXT_STYLE: TextStyle =
{
    align: "left",
    font: 0.5
}

export abstract class MarkerBase
{
    readonly name: string;
    readonly type: MarkerType;

    get flag(): Flag { return Game.flags[this.name]; }
    get pos(): RoomPosition { return this.flag.pos; }

    constructor(name: string, type: MarkerType)
    {
        this.name = name;
        this.type = type;
    }

    abstract run(): void;
}

export abstract class Marker<M extends FlagMemory> extends MarkerBase
{
    get memory(): M { return this.flag.memory as M; }

    constructor(name: string, type: MarkerType)
    {
        super(name, type);
    }
}

class UnknownMarker extends Marker<FlagMemory>
{
    constructor(name: string, type: MarkerType)
    {
        super(name, type);
    }

    run() {}
}

export class InfoMarker extends Marker<FlagMemory>
{
    constructor(name: string, type: MarkerType)
    {
        super(name, type);
    }

    run()
    {
        let room = this.flag.room;

        if (!room) return;

        let visual = room.visual;
        let pos = this.flag.pos;
        let x = pos.x + 1, y = pos.y;

        let energyAvailable = room.energyAvailable || 0;
        let energyCapacity = room.energyCapacityAvailable;
        let wellable = Wellers.maxEnergyPerTick.toFixed(1);
        let performance = "n/a";
        let supplierCount = "n/a";
        let oldest: Creep | undefined = Creeps.oldest;
        let oldestText = oldest ? `${oldest.name} (${oldest.ticksToLive || CREEP_LIFE_TIME})` : "no creeps";

        visual.text(`E: ${energyAvailable} / ${energyCapacity}`, x, y, INFO_MARKER_TEXT_STYLE);
        visual.text(`W / P: ${wellable} / ${performance} (${supplierCount})`, x, y + 1, INFO_MARKER_TEXT_STYLE);
        visual.text(`C: ${oldestText}`, x, y + 2, INFO_MARKER_TEXT_STYLE);
        visual.text(`B: ${Game.cpu.bucket}`, x, y + 3, INFO_MARKER_TEXT_STYLE);
    }
}

export interface GuardMarkerMemory extends FlagMemory
{
    assignee?: string;
}

export class GuardMarker extends Marker<GuardMarkerMemory>
{
    get assignee(): Creep | undefined { return Game.myCreep(this.memory.assignee); }
    set assignee(value: Creep) { this.memory.assignee = value.name; }

    constructor(name: string, type: MarkerType)
    {
        super(name, type);
    }

    run() { }
}

export class Markers
{
    private static _markers: Dictionary<MarkerBase> = {};

    private static _guardMarkers: Vector<GuardMarker> = new Vector();

    static get guardMarkers(): Vector<GuardMarker> { return Markers._guardMarkers.clone(); }
    static get guardMarkerCount(): number { return Markers._guardMarkers.length; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Markers._markers = {};
            Markers._guardMarkers = new Vector();
        }

        const existing: Set<string> = Game.myFlagNames;

        if (Dictionaries.update(Markers._markers, existing, Markers.create))
        {
            const markers = Dictionaries.values(Markers._markers);

            Markers._guardMarkers = markers.filter(m => m.type == MarkerType.Guard) as Vector<GuardMarker>;
        }
    }

    private static create(name: string): MarkerBase
    {
        switch (name.charAt(0))
        {
            case MarkerType.Info: return new InfoMarker(name, MarkerType.Info);
            case MarkerType.Guard: return new GuardMarker(name, MarkerType.Guard);
            default: return new UnknownMarker(name, MarkerType.Unknown);
        }
    }

    @profile
    static run()
    {
        Dictionaries.values(Markers._markers).forEach(m => m.run());
    }
}