import { Dictionaries, Dictionary, MarkerType } from "./Common";
import { Creeps } from "./Creeps";
import { profile } from "./Profiling";
import { Wellers } from "./Wellers";

const INFO_MARKER_TEXT_STYLE: TextStyle =
{
    align: "left",
    font: 0.5
}

export abstract class Marker
{
    readonly name: string; 
    readonly type: MarkerType;

    get flag(): Flag { return Game.flags[this.name]; }

    constructor(name: string, type: MarkerType)
    {
        this.name = name;
        this.type = type;
    }

    abstract run(): void;
}

class UnknownMarker extends Marker
{
    constructor(name: string, type: MarkerType)
    {
        super(name, type);
    }

    run() {}
}

export class InfoMarker extends Marker
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

export class Markers
{
    private static _markers: Dictionary<Marker> = {};

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Markers._markers = {};
        }

        const existing: Set<string> = new Set(Dictionaries.keys(Game.flags));

        Dictionaries.update(Markers._markers, existing, Markers.create);
    }

    private static create(name: string): Marker
    {
        switch (name.charAt(0))
        {
            case MarkerType.Info: return new InfoMarker(name, MarkerType.Info);
            default: return new UnknownMarker(name, MarkerType.Unknown);
        }
    }

    @profile
    static run()
    {
        Dictionaries.values(Markers._markers).forEach(m => m.run());
    }
}