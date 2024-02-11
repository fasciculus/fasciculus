import { Objects } from "../es/object";

export class Positions
{
    private static around(this: RoomPosition, range?: number): Array<RoomPosition>
    {
        if (range === undefined) range = 1;
        if (range < 1) return new Array();

        const result: Array<RoomPosition> = new Array();
        const name: string = this.roomName;
        const x0: number = this.x;
        const y0: number = this.y;

        for (let x = x0 - range, xe = x0 + range; x <= xe; ++x)
        {
            if (x < 0 || x > 49) continue;

            for (let y = y0 - range, ye = y0 + range; y <= ye; ++y)
            {
                if (y < 0 || y > 49) continue;
                if (x == x0 && y == y0) continue;

                result.push(new RoomPosition(x, y, name));
            }
        }

        return result;
    }

    static setup()
    {
        Objects.setFunction(RoomPosition.prototype, "around", Positions.around);
    }
}