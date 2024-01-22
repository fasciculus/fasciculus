import { MarkerType } from "./Enums";
import { GameWrap } from "./GameWrap";
import { Statistics } from "./Statistics";
import { Utils } from "./Utils";

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
        let visual = this.flag.room?.visual;

        if (!visual) return;

        let pos = this.flag.pos;
        let x = pos.x, y = pos.y;

        let welled = Utils.round(Statistics.welled, 1);
        let supplied = Utils.round(Statistics.supplied, 1);

        visual.text(`W / S: ${welled} / ${supplied}`, x + 1, y, INFO_MARKER_TEXT_STYLE);
        visual.text(`B: ${Game.cpu.bucket}`, x + 1, y + 1, INFO_MARKER_TEXT_STYLE);
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