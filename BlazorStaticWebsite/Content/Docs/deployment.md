# How to deploy 

Since the output of BlazorStatic are just plan .html files and assets, you can deploy it fairly easily. You can use GitHub Pages, Azure Static Web Apps, Netlify, ...

## GitHub Pages

- Build and run the app
- Use `JamesIves/github-pages-deploy-action@v4` to publish the output folder to GitHub Pages. The way it works is that it will create orphaned branch `gh-pages` and push the content of the output folder there. 

    ```yml
    - name: Run webapp and generate static files
    run: |
        dotnet run --project path/to/yourProject.csproj  --configuration Release

    - name: Deploy to GitHub Pages
    uses: JamesIves/github-pages-deploy-action@v4
    with:
        folder: path/to/output
    ```
- Set the GitHub Pages to use `gh-pages` branch as source. 
  ![setup github pages](media/deployement/img.png)
- Check the whole pipeline [here](https://github.com/tesar-tech/BlazorStaticMinimalBlog/blob/master/.github/workflows/publish-to-gh-pages.yml)

## Netlify 

I am not certain this is the best approach, but it is definately working

- Build and run the app in CI/CD pipeline
- Create orphaned branch (e.g. `netlify-deploy`) and push the output folder there.
- Go to Netlify and target the deployment to the `netlify-deploy` branch.
- Check the whole pipeline [here](https://github.com/tesar-tech/zodoc/blob/master/.github/workflows/publish-zodoc.yml) (mainly the last steps).

## Azure Static Web Apps
[**TODO**](https://github.com/tesar-tech/BlazorStatic/issues/1) 
Shouldn't be complicated, but I haven't tried it yet.

