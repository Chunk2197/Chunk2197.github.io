GitHub Pages setup checklist for `davis.diy`

1. DNS configuration
   - For an apex domain (davis.diy): add A records pointing to GitHub Pages IPs:
     - `185.199.108.153`
     - `185.199.109.153`
     - `185.199.110.153`
     - `185.199.111.153`
   - For a `www` subdomain: add a CNAME record for `www` pointing to `Chunk2197.github.io`.

2. Add the custom domain to the repository
   - Ensure the repository root `CNAME` file contains exactly `davis.diy` (done).

3. GitHub Pages settings
   - In repository Settings → Pages: set Source to `gh-pages` branch (root) once the branch exists.
   - Ensure GitHub will use the `CNAME` in the branch and attempt to provision TLS.

4. TLS and HTTPS
   - GitHub will attempt to provision HTTPS for the domain if DNS is configured correctly.
   - If TLS fails, verify DNS propagation and that the `CNAME` is present in the `gh-pages` branch.

5. Troubleshooting
   - If you control multiple domains and want redirects, set them up at your DNS provider (GitHub only accepts one custom domain per repo via `CNAME`).
   - If `davis.diy` is a non-public or local TLD, TLS provisioning may not work; consider using a public TLD or using a reverse proxy for local access.

6. Next steps
   - After you push to `master`, the workflow will build and push the `output` to `gh-pages`. Wait a few minutes then check Settings → Pages for the site status.
   - If the site doesn't appear, check the Actions run logs and the `gh-pages` branch contents to verify `index.html` and `CNAME` are present.
   - The RSS feed will be available at `https://davis.diy/feed.xml` after deployment.
