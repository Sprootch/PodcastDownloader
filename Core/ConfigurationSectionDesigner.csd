<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="690d6f50-3960-4859-9123-a5bef3b75b2d" namespace="Core" xmlSchemaNamespace="urn:Core" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="PodcastDownloaderConfiguration" codeGenOptions="Singleton" xmlSectionName="podcastDownloaderConfiguration">
      <elementProperties>
        <elementProperty name="Proxy" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="proxy" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/ProxyConfigurationElement" />
          </type>
        </elementProperty>
        <elementProperty name="MusicPlayer" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="musicPlayer" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/MusicPlayerElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="ProxyConfigurationElement">
      <elementProperties>
        <elementProperty name="Address" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="address" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/ProxyAddressElement" />
          </type>
        </elementProperty>
        <elementProperty name="Security" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="security" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/ProxySecurityElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="ProxySecurityElement">
      <attributeProperties>
        <attributeProperty name="UserName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="userName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Password" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="password" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="ProxyAddressElement">
      <attributeProperties>
        <attributeProperty name="Url" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="url" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Port" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/Int32" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="MusicPlayerElement">
      <attributeProperties>
        <attributeProperty name="Path" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="path" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/690d6f50-3960-4859-9123-a5bef3b75b2d/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>