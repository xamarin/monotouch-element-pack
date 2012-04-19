all:

update-docs:
	MONO_PATH=/Developer/MonoTouch/usr/lib/mono/2.1/ mdoc update ElementPack/bin/Debug/ElementPack.dll --out docs
