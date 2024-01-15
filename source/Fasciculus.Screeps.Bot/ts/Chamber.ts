
import { Territory } from "./Territory";

export class Chamber
{
    private _territory?: Territory;

    readonly room: Room;

    constructor(room: Room)
    {
        this.room = room;
    }

    get name(): string { return this.room.name; }

    get territory(): Territory { return this._territory || (this._territory = new Territory(this.room)); }
}