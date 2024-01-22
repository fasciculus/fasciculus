import { MarkerType } from "./Enums";
import { GameWrap } from "./GameWrap";
import { SUPPLIER_PERFORMANCE_FACTOR } from "./Spawning";
import { Suppliers } from "./Suppliers";
import { Utils } from "./Utils";
import { Wellers } from "./Wellers";

const INFO_MARKER_TEXT_STYLE: TextStyle =
{
    align: "left",
    font: 0.5
}

export class Marker
{
    readonly flag: Flag;
    readonly type: MarkerType;

    constructor(flag: Flag)
    {
        this.flag = flag;
        this.type = Marker.typeOf(flag);
    }

    static typeOf(flag: Flag): MarkerType
    {
        let name = flag.name;

        if (name.length == 0) return MarkerType.Unknown

        switch (flag.name.charAt(0))
        {
            case MarkerType.Info: return MarkerType.Info;
            default: return MarkerType.Unknown;
        }
    }

    run()
    {
        switch (this.type)
        {
            case MarkerType.Info: this.runInfo(); break;
        }
    }

    private runInfo()
    {
        let room = this.flag.room;

        if (!room) return;

        let visual = room.visual;
        let pos = this.flag.pos;
        let x = pos.x + 1, y = pos.y;

        let energyAvailable = room.energyAvailable;
        let energyCapacity = room.energyCapacityAvailable;
        let wellable = Utils.round(Wellers.maxEnergyPerTick, 1);
        let performance = Utils.round(Suppliers.performance * SUPPLIER_PERFORMANCE_FACTOR, 1);
        let supplierCount = Suppliers.count;

        visual.text(`E: ${energyAvailable} / ${energyCapacity}`, x, y, INFO_MARKER_TEXT_STYLE);
        visual.text(`W / P: ${wellable} / ${performance} (${supplierCount})`, x, y + 1, INFO_MARKER_TEXT_STYLE);
        visual.text(`B: ${Game.cpu.bucket}`, x, y + 2, INFO_MARKER_TEXT_STYLE);
    }
}

export class Markers
{
    private static _all: Marker[] = [];

    static initialize()
    {
        Markers._all = GameWrap.myFlags.map(f => new Marker(f));
    }

    static run()
    {
        Markers._all.forEach(m => m.run());
    }
}