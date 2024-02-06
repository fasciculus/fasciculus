import { Objects } from "./types";

export class ScreepsController
{
    private static safe(this: StructureController): boolean
    {
        if (this.my) return true;

        const reservation: ReservationDefinition | undefined = this.reservation;

        if (!reservation) return true;

        return reservation.username == Game.username;
    }

    static setup()
    {
        Objects.setGetter(StructureController.prototype, "safe", ScreepsController.safe)
    }
}