
import { getRooms } from "./room";

export function getControllers(): StructureController[]
{
    return getRooms().map(r => r.controller).filter(c => c !== undefined) as StructureController[];
}

export function getMyControllers(): StructureController[]
{
    return getControllers().filter(c => c.my);
}