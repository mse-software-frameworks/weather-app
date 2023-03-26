# Generates Avro classes for each schemafile located in the directory "Schemafiles"
Get-ChildItem -Path "Schemafiles" | ForEach-Object {

	$filename = $_.Name

	dotnet tool run avrogen -s "Schemafiles\$filename" .\ --namespace "weather.serialization.avro:WeatherApp.Kafka.Schemas.Avro" --skip-directories

	Write-Output "- $filename processed successfully"
}