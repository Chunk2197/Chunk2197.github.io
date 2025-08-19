Statiq.Web scaffold

This repository contains a minimal `Statiq.Web` console application in the `StatiqSite` folder.

Build & run locally:

```powershell
# from repository root
cd StatiqSite
dotnet restore
dotnet run --project . -- preview
```

This will generate the static site into the `output` folder by default and serve it at `http://localhost:5080`.

To build a static output without preview:

```powershell
dotnet run --project .
```

To publish to GitHub Pages, use the included GitHub Actions workflow (it builds the site and pushes the `output` folder to the `gh-pages` branch).

Notes:

- The `CNAME` file in the repository root contains the custom domain: `davis.diy`.
- Edit content in `StatiqSite/input/` (Markdown files) and templates in the Statiq project to customize the site.
