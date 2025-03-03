# Bee.Net

## Overview

Bee.Net is a .Net client for the [Bee Swarm](https://github.com/ethersphere/bee) node.

With this client you can consume public and debug api of any Bee node exposing them.

This includes also some useful to calculate data chunking, and postage batch usage.

## Packages

There are several NuGet packages available for Bee.Net. Here's a brief description of each:

1. **Bee.Net.Client**: This package contains a client that is designed to connect to and work with Bee Swarm. It
   simplifies the interaction with Bee nodes by providing high-level methods and classes.

2. **Bee.Net.Core**: This package contains base models and useful tools to work with Swarm.
   Models represent the core entities in the Swarm world and provide a foundation for interacting with the Bee Swarm.
   Tools include utilities for data chunking, calculating postage batch usage, fork with feeds, and other helpful 
   operations that enhance your experience when working with Swarm.

3. **BeeNet.Core.AspNet**: This package helps to register Bee.Net Core services and converters with Asp.Net

## Package repositories

You can get latest public releases from Nuget.org feed. Here you can see our [published packages](https://www.nuget.org/profiles/etherna).

If you'd like to work with latest internal releases, you can use our [custom feed](https://www.myget.org/F/etherna/api/v3/index.json) (NuGet V3) based on MyGet.

## Issue reports

If you've discovered a bug, or have an idea for a new feature, please report it to our issue manager based on Jira https://etherna.atlassian.net/projects/BNET.

Detailed reports with stack traces, actual and expected behaviours are welcome.

## Questions? Problems?

For questions or problems please write an email to [info@etherna.io](mailto:info@etherna.io).

## License

![LGPL Logo](https://www.gnu.org/graphics/lgplv3-with-text-154x68.png)

We use the GNU Lesser General Public License v3 (LGPL-3.0) for this project.
If you require a custom license, you can contact us at [license@etherna.io](mailto:license@etherna.io).
