using DalvikUWPCSharp.Disassembly.APKParser.bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalvikUWPCSharp.Disassembly.APKParser.parser
{
    public class CertificateParser
    {
        //This needs Aniversary Update or newer to run :(

        /*private byte[] data;

        private List<CertificateMeta> certificateMetas;

        public CertificateParser(byte[] data)
        {
            this.data = data;
        }*/

        /**
         * get certificate info
         *
         * @throws IOException
         * @throws CertificateEncodingException
         */
        /*public void parse()
        {

        PKCS7 pkcs7 = new PKCS7(data);
        X509Certificate[] certificates = pkcs7.getCertificates();
        certificateMetas = new ArrayList<>();
        foreach (X509Certificate certificate in certificates)
            {
            CertificateMeta certificateMeta = new CertificateMeta();
        certificateMetas.Add(certificateMeta);

            byte[] bytes = certificate.getEncoded();
        String certMd5 = md5Digest(bytes);
        String publicKeyString = byteToHexString(bytes);
        String certBase64Md5 = md5Digest(publicKeyString);
        certificateMeta.setData(bytes);
            certificateMeta.setCertBase64Md5(certBase64Md5);
            certificateMeta.setCertMd5(certMd5);
            certificateMeta.setStartDate(certificate.getNotBefore());
            certificateMeta.setEndDate(certificate.getNotAfter());
            certificateMeta.setSignAlgorithm(certificate.getSigAlgName());
            certificateMeta.setSignAlgorithmOID(certificate.getSigAlgOID());
            }
        }


private String md5Digest(byte[] input)
{
    MessageDigest digest = getDigest("Md5");
    digest.update(input);
        return getHexString(digest.digest());
}

private String md5Digest(String input)
{
    MessageDigest digest = getDigest("Md5");
    digest.update(input.getBytes(StandardCharsets.UTF_8));
        return getHexString(digest.digest());
}

private String byteToHexString(byte[] bArray)
{
    StringBuilder sb = new StringBuilder(bArray.length);
    String sTemp;
    for (byte aBArray : bArray)
    {
        sTemp = Integer.toHexString(0xFF & (char)aBArray);
        if (sTemp.length() < 2)
        {
            sb.append(0);
        }
        sb.append(sTemp.toUpperCase());
    }
    return sb.toString();
}

private String getHexString(byte[] digest)
{
    BigInteger bi = new BigInteger(1, digest);
    return String.format("%032x", bi);
}

private MessageDigest getDigest(String algorithm)
{
    try
    {
        return MessageDigest.getInstance(algorithm);
    }
    catch (NoSuchAlgorithmException e)
    {
        throw new RuntimeException(e.getMessage());
    }
}

public List<CertificateMeta> getCertificateMetas()
{
    return certificateMetas;
}*/
    }
}
