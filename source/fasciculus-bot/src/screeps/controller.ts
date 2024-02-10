import { Objects } from "../es/object";

declare global
{
    interface StructureController
    {
        get safe(): boolean;
    }
}

export class ControllerExt
{
    private static safe(this: StructureController): boolean
    {
        if (this.my) return true;

        const reservation: ReservationDefinition | undefined = this.reservation;

        if (!reservation) return true;

        return reservation.username == "rhjoerg"; // Game.username;
    }

    static setup()
    {
        Objects.setGetter(StructureController.prototype, "safe", ControllerExt.safe);
    }
}