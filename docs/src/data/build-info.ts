import { createRequire } from "node:module";
import { execSync } from "node:child_process";

const require = createRequire(import.meta.url);

const REPO = "https://github.com/IEvangelist/pwned-client";

function packageVersion(name: string): string {
  try {
    return (require(`${name}/package.json`) as { version: string }).version;
  } catch {
    return "unknown";
  }
}

interface Commit {
  short: string;
  full: string | null;
  url: string | null;
}

function resolveCommit(): Commit {
  const fromEnv = process.env.GITHUB_SHA?.trim();
  const sha =
    fromEnv ||
    (() => {
      try {
        return execSync("git rev-parse HEAD", {
          stdio: ["ignore", "pipe", "ignore"],
        })
          .toString()
          .trim();
      } catch {
        return "";
      }
    })();

  if (!sha) {
    return { short: "dev", full: null, url: null };
  }

  return {
    short: sha.slice(0, 7),
    full: sha,
    url: `${REPO}/commit/${sha}`,
  };
}

export interface BuildInfo {
  repo: string;
  commit: Commit;
  versions: { label: string; value: string }[];
}

export const buildInfo: BuildInfo = {
  repo: REPO,
  commit: resolveCommit(),
  versions: [
    { label: "Astro", value: packageVersion("astro") },
    { label: "TypeScript", value: packageVersion("typescript") },
    { label: "Tailwind", value: packageVersion("tailwindcss") },
    { label: "Shiki", value: packageVersion("shiki") },
    { label: "Node", value: process.version.replace(/^v/, "") },
  ],
};
