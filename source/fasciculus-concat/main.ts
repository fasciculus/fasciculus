import * as fs from 'fs';
import * as path from "path";

class Source
{
    readonly absolutePath: string;
    readonly relativePath: string;
    readonly key: string;
    readonly parent: string;
    readonly lines: Array<string>;

    constructor(sourceDirectory: string, absolutePath: string)
    {
        this.absolutePath = absolutePath;
        this.relativePath = path.relative(sourceDirectory, absolutePath);
        this.key = this.relativePath.slice(0, -3).replaceAll("\\", "/");
        this.parent = Source.getParent(this.key);
        this.lines = fs.readFileSync(absolutePath, { encoding: "utf8" }).split(/\r?\n/);
    }

    private static getParent(key: string): string
    {
        const index: number = key.lastIndexOf("/");

        return index < 0 ? "" : key.substring(0, index);
    }
}

class SourceImport
{
    readonly source: Source;
    readonly line: string;
    readonly unresolved: string;
    readonly resolved: string;

    constructor(source: Source, line: string, keys: Set<string>)
    {
        this.source = source;
        this.line = line;
        this.unresolved = SourceImport.getUnresolved(line);
        this.resolved = SourceImport.resolve(source, this.unresolved, keys);
    }

    private static resolve(source: Source, unresolved: string, keys: Set<string>): string
    {
        const resolved: string = path.join(source.parent, unresolved).replaceAll("\\", "/");

        return keys.has(resolved) ? resolved : "";
    }

    private static getUnresolved(line: string): string
    {
        if (line.trim().endsWith(";")) line = line.substring(0, line.length - 1).trim();

        if (line.endsWith("\""))
        {
            line = line.substring(0, line.length - 1).trim();

            const index = line.lastIndexOf("\"");

            if (index < 0) return "";

            return line.substring(index + 1);
        }
        else if (line.endsWith("'"))
        {
            line = line.substring(0, line.length - 1).trim();

            const index = line.lastIndexOf("'");

            if (index < 0) return "";

            return line.substring(index + 1);
        }
        else
        {
            return "";
        }
    }
}

class SourceImports
{
    readonly source: Source;
    readonly imports: Array<SourceImport>;

    constructor(source: Source, keys: Set<string>)
    {
        this.source = source;
        this.imports = source.lines.filter(l => l.trimStart().startsWith("import"))
            .map(l => new SourceImport(source, l, keys));
    }
}

const args: Array<string> = process.argv.slice(2);
const sourceDirectory = path.join(__dirname, args[0]);

console.log(`source directory = ${sourceDirectory}`);

const sources: Array<Source> = fs.readdirSync(sourceDirectory, { encoding: "utf8", recursive: true, withFileTypes: true })
    .filter(e => e.isFile())
    .map(e => path.join(e.path, e.name))
    .map(f => new Source(sourceDirectory, f));

console.log(`processing ${sources.length} files`);
sources.forEach(s => console.log(`  ${s.relativePath} (key: "${s.key}", parent: "${s.parent}")`));

const sourceKeys: Set<string> = new Set(sources.map(s => s.key));
const sourceImports: Array<SourceImports> = sources.map(s => new SourceImports(s, sourceKeys));

for (const si of sourceImports)
{
    console.log(`${si.source.key} imports:`);
    si.imports.forEach(i => console.log(` "${i.unresolved}" => "${i.resolved}"`));
}
