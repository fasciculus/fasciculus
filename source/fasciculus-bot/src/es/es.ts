import { Arrays } from "./array";
import { Maps } from "./map";
import { Sets } from "./set";

export class ES
{
    static setup(): void
    {
        Arrays.setup();
        Sets.setup();
        Maps.setup();
    }
}