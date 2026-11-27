# PerformanceImprovements in SE-DX2.0Code

This folder is a `git subtree` of [DraconisCluster/se-performance-improvements](https://github.com/DraconisCluster/se-performance-improvements)
(our fork of viktor-ferenczi/se-performance-improvements, which no longer ships a Torch plugin). Only the Torch plugin
is built here: `TorchPlugin/TorchPlugin.csproj` plus the `Shared` project. `ClientPlugin`, `DedicatedPlugin` and
`Tests` are kept as imported but are not in `Expanse2.sln`.

## What differs from the fork

- **Plugin GUID `f4b2c663-8aa9-4bec-aa91-f12dc224a8e0`** (upstream: `c2cf3ed2-c6ac-4dbd-ab9a-613a1ef67784`).
  torchapi.com doesn't know this GUID, so Torch's plugin updater can't replace our build with upstream's, and an old
  zip can't shadow the managed folder. Torch loads folder plugins whether or not their GUID is in `Torch.cfg` (only
  zips are filtered by that list) and never auto-updates them, so this GUID doesn't need listing anywhere.
  **Migration hazard:** because the GUIDs differ, a server that still has `Performance Improvements.zip` with
  `c2cf3ed2…` listed (by `Torch.cfg` or a Nexus config group) loads **both** copies. Remove the old GUID from the
  Nexus groups and `Torch.cfg`, and delete the zip, before the restart that picks up the folder.
- **Version `1.11.22.x`:** the fourth part counts our builds on top of upstream 1.11.22. Bump `<Version>` in
  `manifest.xml` and `AssemblyVersion`/`FileVersion` in the csproj together. CI pushes the manifest version as the
  Harbor tag and refuses to move an existing version tag to new content.
- **`manifest.xml` sits at the folder root** (was `TorchPlugin/manifest.xml`), where the workflow reads the version.
- **References come from NuGet reference packages** (Torch, SE 1.211.19, Roslyn 2.9.0), not a `Torch` junction.
  When the servers move to a new game version, bump `SpaceEngineersDedicated.ReferenceAssemblies`.
- **`deploy.bat` is removed.** The build writes `plugins/.build/PerformanceImprovements.zip`, containing
  `PerformanceImprovements.dll`/`.pdb`, `0Harmony.dll`, `System.Runtime.CompilerServices.Unsafe.dll` and `manifest.xml`.

## Delivery

The "Build solution" workflow pushes the zip to Harbor as `se-dx2/performanceimprovements`:

- a PR that touches this folder pushes the `pr-<number>` tag;
- a merge to `main` pushes `<version>` and `latest`.

The control plane registers it as private plugin `PerformanceImprovements` (guest folder `PerformanceImprovements`),
mirrors the pinned tag and installs it to `Plugins\PerformanceImprovements\` on each server. See the migration
hazard above before a server's first install.

## Keeping the fork and this folder in step

The fork's `main` carries the same files as this folder and keeps PI's commit history (se-dx2 squash-merges, so the
history isn't here). Make a change in one place, then bring it to the other:

- fork → se-dx2: `git subtree pull --squash --prefix=plugins/PerformanceImprovements https://github.com/DraconisCluster/se-performance-improvements.git main`
- se-dx2 → fork: apply the same change to the fork with a normal commit or PR.

Releases for people outside se-dx2 are GitHub Releases on the fork, with the zip CI pushed to Harbor attached.
