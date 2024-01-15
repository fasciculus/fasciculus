
export class Utils
{
    static nullAsZero(value: number | null | undefined): number
    {
        if (value === null) return 0;
        if (value === undefined) return 0;

        return value;
    }
}