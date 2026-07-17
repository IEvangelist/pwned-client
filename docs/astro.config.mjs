import { fileURLToPath } from "node:url";

const localPreview = process.env.ASTRO_LOCAL_PREVIEW === "1";
const tailwindPlugin = process.env.ASTRO_LOCAL_PREVIEW === "1"
  ? undefined
  : (await import("@tailwindcss/vite")).default();

export default {
  site: "https://ievangelist.github.io",
  base: "/pwned-client",
  trailingSlash: "always",
  vite: {
    plugins: tailwindPlugin ? [tailwindPlugin] : [],
    resolve: localPreview
      ? {
          alias: {
            tailwindcss: fileURLToPath(new URL("./src/styles/tailwind-preview.css", import.meta.url)),
            shiki: fileURLToPath(new URL("./node_modules/shiki/dist/index.mjs", import.meta.url)),
          },
        }
      : undefined,
  },
};
