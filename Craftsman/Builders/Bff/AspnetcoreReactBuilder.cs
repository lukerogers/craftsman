﻿namespace Craftsman.Builders.Bff
{
  using System;
  using System.IO.Abstractions;
  using System.Linq;
  using Enums;
  using Helpers;
  using Models;
  using static Helpers.ConstMessages;

public class AspnetcoreReactBuilder
{
	public static void CreateAspnetcoreReact(string spaDirectory, IFileSystem fileSystem)
    {
      var classPath = ClassPathHelper.BffSpaRootClassPath(spaDirectory, "aspnetcore-react.js");
      var fileText = GetAspnetcoreReactText();
      Utilities.CreateFile(classPath, fileText, fileSystem);
    }

    public static string GetAspnetcoreReactText()
    {
      return @$"// This script configures the .env.development.local file with additional environment variables to configure HTTPS using the ASP.NET Core
// development certificate in the webpack development proxy.

const fs = require('fs');
const path = require('path');

const baseFolder =
	process.env.APPDATA !== undefined && process.env.APPDATA !== ''
		? `${{process.env.APPDATA}}/ASP.NET/https`
		: `${{process.env.HOME}}/.aspnet/https`;

const certificateArg = process.argv
	.map((arg) => arg.match(/--name=(?<value>.+)/i))
	.filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : process.env.npm_package_name;

if (!certificateName) {{
	console.error(
		'Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.'
	);
	process.exit(-1);
}}

const certFilePath = path.join(baseFolder, `${{certificateName}}.pem`);
const keyFilePath = path.join(baseFolder, `${{certificateName}}.key`);

if (!fs.existsSync('.env.development.local')) {{
	fs.writeFileSync(
		'.env.development.local',
		`SSL_CRT_FILE=${{certFilePath}}
SSL_KEY_FILE=${{keyFilePath}}`
	);
}} else {{
	let lines = fs.readFileSync('.env.development.local').toString().split('\n');

	let hasCert,
		hasCertKey = false;
	for (const line of lines) {{
		if (/SSL_CRT_FILE=.*/i.test(line)) {{
			hasCert = true;
		}}
		if (/SSL_KEY_FILE=.*/i.test(line)) {{
			hasCertKey = true;
		}}
	}}
	if (!hasCert) {{
		fs.appendFileSync('.env.development.local', `\nSSL_CRT_FILE=${{certFilePath}}`);
	}}
	if (!hasCertKey) {{
		fs.appendFileSync('.env.development.local', `\nSSL_KEY_FILE=${{keyFilePath}}`);
	}}
}}";
    }
  }
}