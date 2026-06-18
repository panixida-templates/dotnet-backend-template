{{- define "dotnet-template.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "dotnet-template.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{- define "dotnet-template.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "dotnet-template.labels" -}}
helm.sh/chart: {{ include "dotnet-template.chart" . | quote }}
{{ include "dotnet-template.selectorLabels" . }}
app.kubernetes.io/managed-by: {{ .Release.Service | quote }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end -}}

{{- define "dotnet-template.selectorLabels" -}}
app.kubernetes.io/name: {{ include "dotnet-template.name" . | quote }}
app.kubernetes.io/instance: {{ .Release.Name | quote }}
{{- end -}}

{{- define "dotnet-template.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
{{- default (include "dotnet-template.fullname" .) .Values.serviceAccount.name -}}
{{- else -}}
{{- default "default" .Values.serviceAccount.name -}}
{{- end -}}
{{- end -}}

{{- define "dotnet-template.externalSecretsServiceAccountName" -}}
{{- default (printf "%s-external-secrets" (include "dotnet-template.fullname" .)) .Values.externalSecrets.serviceAccount.name -}}
{{- end -}}

{{- define "dotnet-template.secretStoreName" -}}
{{- printf "%s-openbao" (include "dotnet-template.fullname" .) | trunc 63 | trimSuffix "-" -}}
{{- end -}}
