import { Objects } from "../es/object";
import { Cached } from "./cache";

export class ControllerExt
{
    private static _safe: Cached<Map<ControllerId, boolean>> = Cached.simple(() => new Map());

    private static getSafe(id: ControllerId): boolean
    {
        const controller: StructureController | undefined = Game.get(id);

        if (!controller) return false;
        if (controller.my) return true;

        const reservation: ReservationDefinition | undefined = controller.reservation;

        if (!reservation) return true;

        return reservation.username == Game.username;
    }

    private static safe(this: StructureController): boolean
    {
        return ControllerExt._safe.value.ensure(this.id, ControllerExt.getSafe);
    }

    static setup()
    {
        Objects.setGetter(StructureController.prototype, "safe", ControllerExt.safe);
    }
}