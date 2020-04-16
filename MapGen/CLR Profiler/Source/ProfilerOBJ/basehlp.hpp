// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/****************************************************************************************
 * File:
 *  basehlp.hpp
 *
 * Description:
 *  
 *
 *
 ***************************************************************************************/


/***************************************************************************************
 ********************                                               ********************
 ********************          BaseException Implementation         ********************
 ********************                                               ********************
 ***************************************************************************************/

/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/

DECLSPEC
/* public */
BaseException::BaseException( const char *reason ) :
    m_reason( NULL )
{
    SIZE_T length = strlen( reason );
    
    
    m_reason = new char[(length + 1)];
    strcpy( m_reason, reason );

} // ctor


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* virtual public */
BaseException::~BaseException() 
{
    if ( m_reason != NULL )
        delete[] m_reason;

} // dtor


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* virtual public */
void BaseException::ReportFailure()
{
    TEXT_OUTLN( m_reason );
    
} // BaseException::ReportFailure


/***************************************************************************************
 ********************                                               ********************
 ********************            Synchronize Implementation         ********************
 ********************                                               ********************
 ***************************************************************************************/

/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* public */
Synchronize::Synchronize( CRITICAL_SECTION &criticalSection ) : 
    m_block( criticalSection )
{
    EnterCriticalSection( &m_block );
    
} // ctor


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* public */
Synchronize::~Synchronize()
{
    LeaveCriticalSection( &m_block );

} // dtor


/***************************************************************************************
 ********************                                               ********************
 ********************            BASEHELPER Implementation          ********************
 ********************                                               ********************
 ***************************************************************************************/

/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
DWORD BASEHELPER::FetchEnvironment( const char *environment )
{
    DWORD retVal = -1;
    char buffer[MAX_LENGTH];
    
    
    if ( GetEnvironmentVariableA( environment, buffer, MAX_LENGTH ) > 0 )
        retVal = BASEHELPER::GetEnvVarValue( buffer );
                
    
    return retVal;

} // BASEHELPER::FetchEnvironemnt


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
void BASEHELPER::DDebug( char *format, ... )
{
    static DWORD debugShow = BASEHELPER::FetchEnvironment( DEBUG_ENVIRONMENT );
    
    
    if ( (debugShow == 2) || (debugShow == 3) ) 
    {
        va_list args;
        DWORD dwLength;
        char buffer[MAX_LENGTH];
   

        va_start( args, format );    
        dwLength = wvsprintfA( buffer, format, args );

        printf( "%s\n", buffer );
    }
   
} // BASEHELPER::DDebug


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
void BASEHELPER::Display( char *format, ... )
{
    va_list args;
    DWORD dwLength;
    char buffer[MAX_LENGTH];


    va_start( args, format );    
    dwLength = wvsprintfA( buffer, format, args );

    printf( "%s\n", buffer );       
   
} // BASEHELPER::Display


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
void BASEHELPER::LogToFile( char *format, ... )
{
    va_list args;
    static DWORD dwVarValue = BASEHELPER::FetchEnvironment( LOG_ENVIRONMENT );


    va_start( args, format );        
    switch ( dwVarValue )
    {
        case 0x00:
        case 0xFFFFFFFF:
             vprintf( format, args );
             break;


        case 0xFF:
            break;


        default:
            {
                static count = 0;
                static CRITICAL_SECTION criticalSection = { 0 };

                
                if ( count++ == 0 )
                    InitializeCriticalSection( &criticalSection );
                
                
                {
                    FILE *stream;
                    Synchronize guard( criticalSection );
                

                    //
                    // first time create otherwise append
                    //
                    stream = ((count == 1) ? fopen( "output.log", "w" ) : fopen( "output.log", "a+" ));
                    if ( stream != NULL )
                    {
                        vfprintf( stream, format, args );
                        fflush( stream );
                        fclose( stream );
                    }
                    else
                       TEXT_OUTLN( "Unable to open log file" )
                }
            }            
            break;
            
    } // switch

} // BASEHELPER::LogToFile


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
void BASEHELPER::LaunchDebugger( const char *szMsg, const char *szFile, int iLine )
{   
    static DWORD launchDebugger = BASEHELPER::FetchEnvironment( DEBUG_ENVIRONMENT );
    
    
    if ( (launchDebugger >= 1) && (launchDebugger != 0xFFFFFFFF) )
    {       
        char title[MAX_LENGTH];
        char message[MAX_LENGTH];
        
            
        sprintf( message, 
                 "%s\n\n"     \
                 "File: %s\n" \
                 "Line: %d\n",
                 ((szMsg == NULL) ? "FAILURE" : szMsg), 
                 szFile, 
                 iLine );
             
        sprintf( title, 
                 "Test Failure (PID: %d/0x%08x, Thread: %d/0x%08x)      ",
                 GetCurrentProcessId(), 
                 GetCurrentProcessId(), 
                 GetCurrentThreadId(), 
                 GetCurrentThreadId() );
                      
        switch ( MessageBoxA( NULL, 
                              message, 
                              title, 
                              (MB_ABORTRETRYIGNORE | MB_ICONEXCLAMATION) ) )
        {
            case IDABORT:
                TerminateProcess( GetCurrentProcess(), 999 /* bad exit code */ );
                break;


            case IDRETRY:
                _DbgBreak();


            case IDIGNORE:
                break;
                                
        } // switch
    }
    
} // BASEHELPER::LaunchDebugger


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
int BASEHELPER::String2Number( char *number )
{
    WCHAR ch;
    int base; 
    int iIndex = 1;
    BOOL errorOccurred = FALSE;


    // check to see if this is a valid number
    // if the first digit is '0', then this is 
    // a hex or octal number
    if ( number[0] == '0' )
    {
        //
        // hex
        //
        if ( (number[1] == 'x') || (number[1] == 'X') )
        {
            iIndex++;
            
            base = 16;
            while ( (errorOccurred == FALSE) &&
                    ((ch = number[iIndex++]) != '\0') )
            {   
                if ( ((ch >= '0') && (ch <= '9'))  ||
                     ((ch >= 'a') && (ch <= 'f'))  ||
                     ((ch >= 'A') && (ch <= 'F')) )
                {
                    continue;
                }
                
                errorOccurred = TRUE;
            }
        }
        //
        // octal
        //
        else
        {
            base = 8;
            while ( (errorOccurred == FALSE) &&
                    ((ch = number[iIndex++]) != '\0') )
            {   
                if ( (ch >= '0') && (ch <= '7') )
                    continue;
                
                
                errorOccurred = TRUE;
            }
        }
    }
    //
    // decimal
    //
    else
    {
        base = 10;
        while  ( (errorOccurred == FALSE) &&
                 ((ch = number[iIndex++]) != '\0') )
        {   
            if ( (ch >= '0') && (ch <= '9') )
                continue;
            
            
            errorOccurred = TRUE;
        }
    }
    
    
    return ((errorOccurred == TRUE) ? -1 : base);

} // BASEHELPER::String2Number


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */ 
int BASEHELPER::ElementTypeToString( CorElementType elementType, WCHAR *buffer, size_t buflen )
{
    int ret = 0; // success


    switch ( elementType )
    {
        case ELEMENT_TYPE_END:
             wcsncpy( buffer, L"ELEMENT_TYPE_END", buflen );
             break;
        
        case ELEMENT_TYPE_VOID:
             wcsncpy( buffer, L"ELEMENT_TYPE_VOID", buflen );
             break;

        case ELEMENT_TYPE_BOOLEAN:
             wcsncpy( buffer, L"ELEMENT_TYPE_BOOLEAN", buflen );
             break;

        case ELEMENT_TYPE_CHAR:
             wcsncpy( buffer, L"ELEMENT_TYPE_CHAR", buflen );
             break;

        case ELEMENT_TYPE_I1:
             wcsncpy( buffer, L"ELEMENT_TYPE_I1", buflen );
             break;

        case ELEMENT_TYPE_U1:
             wcsncpy( buffer, L"ELEMENT_TYPE_U1", buflen );
             break;

        case ELEMENT_TYPE_I2:
             wcsncpy( buffer, L"ELEMENT_TYPE_I2", buflen );
             break;

        case ELEMENT_TYPE_U2:
             wcsncpy( buffer, L"ELEMENT_TYPE_U2", buflen );
             break;

        case ELEMENT_TYPE_I4:
             wcsncpy( buffer, L"ELEMENT_TYPE_I4", buflen );
             break;

        case ELEMENT_TYPE_U4:
             wcsncpy( buffer, L"ELEMENT_TYPE_U4", buflen );
             break;

        case ELEMENT_TYPE_I8:
             wcsncpy( buffer, L"ELEMENT_TYPE_I8", buflen );
             break;

        case ELEMENT_TYPE_U8:
             wcsncpy( buffer, L"ELEMENT_TYPE_U8", buflen );
             break;

        case ELEMENT_TYPE_R4:
             wcsncpy( buffer, L"ELEMENT_TYPE_R4", buflen );
             break;

        case ELEMENT_TYPE_R8:
             wcsncpy( buffer, L"ELEMENT_TYPE_R8", buflen );
             break;

        case ELEMENT_TYPE_STRING:
             wcsncpy( buffer, L"ELEMENT_TYPE_STRING", buflen );
             break;

        case ELEMENT_TYPE_PTR:
             wcsncpy( buffer, L"ELEMENT_TYPE_PTR", buflen );
             break;

        case ELEMENT_TYPE_BYREF:
             wcsncpy( buffer, L"ELEMENT_TYPE_BYREF", buflen );
             break;

        case ELEMENT_TYPE_VALUETYPE:
             wcsncpy( buffer, L"ELEMENT_TYPE_VALUETYPE", buflen );
             break;

        case ELEMENT_TYPE_CLASS:
             wcsncpy( buffer, L"ELEMENT_TYPE_CLASS", buflen );
             break;

        case ELEMENT_TYPE_ARRAY:
             wcsncpy( buffer, L"ELEMENT_TYPE_ARRAY", buflen );
             break;

        case ELEMENT_TYPE_TYPEDBYREF:
             wcsncpy( buffer, L"ELEMENT_TYPE_TYPEDBYREF", buflen );
             break;

        case ELEMENT_TYPE_I:
             wcsncpy( buffer, L"ELEMENT_TYPE_I", buflen );
             break;

        case ELEMENT_TYPE_U:
             wcsncpy( buffer, L"ELEMENT_TYPE_U", buflen );
             break;

        case ELEMENT_TYPE_FNPTR:
             wcsncpy( buffer, L"ELEMENT_TYPE_FNPTR", buflen );
             break;

        case ELEMENT_TYPE_OBJECT:
             wcsncpy( buffer, L"ELEMENT_TYPE_OBJECT", buflen );
             break;

        case ELEMENT_TYPE_SZARRAY:
             wcsncpy( buffer, L"ELEMENT_TYPE_SZARRAY", buflen );
             break;

        case ELEMENT_TYPE_CMOD_REQD:
             wcsncpy( buffer, L"ELEMENT_TYPE_CMOD_REQD", buflen );
             break;

        case ELEMENT_TYPE_CMOD_OPT:
             wcsncpy( buffer, L"ELEMENT_TYPE_CMOD_OPT", buflen );
             break;

        case ELEMENT_TYPE_MAX:
             wcsncpy( buffer, L"ELEMENT_TYPE_MAX", buflen );
             break;

        case ELEMENT_TYPE_MODIFIER:
             wcsncpy( buffer, L"ELEMENT_TYPE_MODIFIER", buflen );
             break;

        case ELEMENT_TYPE_SENTINEL:
             wcsncpy( buffer, L"ELEMENT_TYPE_SENTINEL", buflen );
             break;

        case ELEMENT_TYPE_PINNED:
             wcsncpy( buffer, L"ELEMENT_TYPE_PINNED", buflen );
             break;

        default:
             ret = -1;
             wcsncpy( buffer, L"<UNKNOWN>", buflen );
             break;
    }
    buffer[buflen-1] = L'\0';

    return ret;

} // BASEHELPER::ElementTypeToString


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
DWORD BASEHELPER::GetEnvVarValue( char *value )
{   
    DWORD retValue = -1;
    int base = BASEHELPER::String2Number( value );


    if ( base != -1 )
        retValue = (DWORD)strtoul( value, NULL, base );


    return retValue;

} // BASEHELPER::GetEnvVarValue


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
PCCOR_SIGNATURE BASEHELPER::ParseElementType( IMetaDataImport *pMDImport,
                                              PCCOR_SIGNATURE signature, 
                                              char *buffer )
{   
    switch ( *signature++ ) 
    {   
        case ELEMENT_TYPE_VOID:
            strcat( buffer, "void" );   
            break;                  
        
        
        case ELEMENT_TYPE_BOOLEAN:  
            strcat( buffer, "bool" );   
            break;  
        
        
        case ELEMENT_TYPE_CHAR:
            strcat( buffer, "wchar" );  
            break;      
                    
        
        case ELEMENT_TYPE_I1:
            strcat( buffer, "int8" );   
            break;      
        
        
        case ELEMENT_TYPE_U1:
            strcat( buffer, "unsigned int8" );  
            break;      
        
        
        case ELEMENT_TYPE_I2:
            strcat( buffer, "int16" );  
            break;      
        
        
        case ELEMENT_TYPE_U2:
            strcat( buffer, "unsigned int16" ); 
            break;          
        
        
        case ELEMENT_TYPE_I4:
            strcat( buffer, "int32" );  
            break;
            
        
        case ELEMENT_TYPE_U4:
            strcat( buffer, "unsigned int32" ); 
            break;      
        
        
        case ELEMENT_TYPE_I8:
            strcat( buffer, "int64" );  
            break;      
        
        
        case ELEMENT_TYPE_U8:
            strcat( buffer, "unsigned int64" ); 
            break;      
        
        
        case ELEMENT_TYPE_R4:
            strcat( buffer, "float32" );    
            break;          
        
        
        case ELEMENT_TYPE_R8:
            strcat( buffer, "float64" );    
            break;      
        
        
        case ELEMENT_TYPE_U:
            strcat( buffer, "unsigned int" );   
            break;       
        
        
        case ELEMENT_TYPE_I:
            strcat( buffer, "int" );    
            break;            
        
        
        case ELEMENT_TYPE_OBJECT:
            strcat( buffer, "Object" ); 
            break;       
        
        
        case ELEMENT_TYPE_STRING:
            strcat( buffer, "String" ); 
            break;       
        
        
        case ELEMENT_TYPE_TYPEDBYREF:
            strcat( buffer, "refany" ); 
            break;                     

        case ELEMENT_TYPE_CLASS:    
        case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_CMOD_OPT:
            {   
                mdToken token;  
                char classname[MAX_LENGTH];


                classname[0] = '\0';
                signature += CorSigUncompressToken( signature, &token ); 
                if ( TypeFromToken( token ) != mdtTypeRef )
                {
                    HRESULT hr;
                    WCHAR zName[MAX_LENGTH];
                    
                    
                    hr = pMDImport->GetTypeDefProps( token, 
                                                     zName,
                                                     MAX_LENGTH,
                                                     NULL,
                                                     NULL,
                                                     NULL );
                    if ( SUCCEEDED( hr ) )
                        wcstombs( classname, zName, MAX_LENGTH );
                }
                    
                strcat( buffer, classname );        
            }
            break;  
        
        
        case ELEMENT_TYPE_SZARRAY:   
            signature = BASEHELPER::ParseElementType( pMDImport, signature, buffer ); 
            strcat( buffer, "[]" );
            break;      
        
        
        case ELEMENT_TYPE_ARRAY:    
            {   
                ULONG rank;
                

                signature = BASEHELPER::ParseElementType( pMDImport, signature, buffer );                 
                rank = CorSigUncompressData( signature );                                                   
                if ( rank == 0 ) 
                    strcat( buffer, "[?]" );

                else 
                {
                    ULONG *lower;   
                    ULONG *sizes;   
                    ULONG numsizes; 
                    ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
                    
                                         
                    lower = (ULONG *)_alloca( arraysize );                                                        
                    memset( lower, 0, arraysize ); 
                    sizes = &lower[rank];

                    numsizes = CorSigUncompressData( signature );   
                    if ( numsizes <= rank )
                    {
                        ULONG numlower;
                        
                        
                        for ( ULONG i = 0; i < numsizes; i++ )  
                            sizes[i] = CorSigUncompressData( signature );   
                        
                        
                        numlower = CorSigUncompressData( signature );   
                        if ( numlower <= rank )
                        {
                            for ( i = 0; i < numlower; i++) 
                                lower[i] = CorSigUncompressData( signature ); 
                            
                            
                            strcat( buffer, "[" );  
                            for ( i = 0; i < rank; i++ )    
                            {   
                                if ( (sizes[i] != 0) && (lower[i] != 0) )   
                                {   
                                    if ( lower[i] == 0 )    
                                        sprintf ( buffer, "%d", sizes[i] ); 

                                    else    
                                    {   
                                        sprintf( buffer, "%d", lower[i] );  
                                        strcat( buffer, "..." );    
                                        
                                        if ( sizes[i] != 0 )    
                                            sprintf( buffer, "%d", (lower[i] + sizes[i] + 1) ); 
                                    }   
                                }
                                    
                                if ( i < (rank - 1) ) 
                                    strcat( buffer, "," );  
                            }   
                            
                            strcat( buffer, "]" );  
                        }                       
                    }
                }
            } 
            break;  

        
        case ELEMENT_TYPE_PINNED:
            signature = BASEHELPER::ParseElementType( pMDImport, signature, buffer ); 
            strcat( buffer, "pinned" ); 
            break;  
         
        
        case ELEMENT_TYPE_PTR:   
            signature = BASEHELPER::ParseElementType( pMDImport, signature, buffer ); 
            strcat( buffer, "*" );  
            break;   
        
        
        case ELEMENT_TYPE_BYREF:   
            signature = BASEHELPER::ParseElementType( pMDImport, signature, buffer ); 
            strcat( buffer, "&" );  
            break;              


        default:    
        case ELEMENT_TYPE_END:  
        case ELEMENT_TYPE_SENTINEL: 
            strcat( buffer, "<UNKNOWN>" );  
            break;                                                              
                            
    } // switch 
    
    
    return signature;

} // BASEHELPER::ParseElementType


DECLSPEC
/* static public */
HRESULT BASEHELPER::GetClassName(IMetaDataImport *pMDImport, mdToken classToken, WCHAR className[])
{
    DWORD dwTypeDefFlags = 0;
    HRESULT hr = S_OK;
    hr = pMDImport->GetTypeDefProps( classToken, 
                                     className, 
                                     MAX_LENGTH,
                                     NULL, 
                                     &dwTypeDefFlags, 
                                     NULL ); 
    if ( FAILED( hr ) )
    {
        return hr;
    }
    if (IsTdNested(dwTypeDefFlags))
    {
//      printf("%S is a nested class\n", className);
        mdToken enclosingClass = mdTokenNil;
        hr = pMDImport->GetNestedClassProps(classToken, &enclosingClass);
        if ( FAILED( hr ) )
        {
            return hr;
        }
//      printf("Enclosing class for %S is %d\n", className, enclosingClass);
        hr = GetClassName(pMDImport, enclosingClass, className);
//      printf("Enclosing class name %S\n", className);
        if (FAILED(hr))
            return hr;
        size_t length = wcslen(className);
        if (length + 2 < MAX_LENGTH)
        {
            className[length++] = '.';
            hr = pMDImport->GetTypeDefProps( classToken, 
                                            className + length, 
                                            (ULONG)(MAX_LENGTH - length),
                                            NULL, 
                                            NULL, 
                                            NULL );
            if ( FAILED( hr ) )
            {
                return hr;
            }
//          printf("%S is a nested class\n", className);
        }
    }
    return hr;
}

/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
HRESULT BASEHELPER::GetFunctionProperties( ICorProfilerInfo *pPrfInfo,
                                           FunctionID functionID,
                                           BOOL *isStatic,
                                           ULONG *argCount,
                                           WCHAR *returnTypeStr, 
                                           size_t returnTypeStrLen,
                                           WCHAR *functionParameters,
                                           size_t functionParametersLen,
                                           WCHAR *functionName,
                                           size_t functionNameLen )
{
    HRESULT hr = E_FAIL; // assume success
    
        
    if ( functionID != NULL )
    {
        mdToken token;
        ClassID classID;
        IMetaDataImport *pMDImport = NULL;      
        WCHAR funName[MAX_LENGTH] = L"UNKNOWN";
                
                
        
        //
        // Get the classID 
        //
        hr = pPrfInfo->GetFunctionInfo( functionID,
                                        &classID,
                                        NULL,
                                        NULL );
        if ( SUCCEEDED( hr ) )
        {
            //
            // Get the MetadataImport interface and the metadata token 
            //
            hr = pPrfInfo->GetTokenAndMetaDataFromFunction( functionID, 
                                                            IID_IMetaDataImport, 
                                                            (IUnknown **)&pMDImport,
                                                            &token );
            if ( SUCCEEDED( hr ) )
            {
                hr = pMDImport->GetMethodProps( token,
                                                NULL,
                                                funName,
                                                MAX_LENGTH,
                                                0,
                                                0,
                                                NULL,
                                                NULL,
                                                NULL, 
                                                NULL );
                if ( SUCCEEDED( hr ) )
                {
                    mdTypeDef classToken = NULL;
                    WCHAR className[MAX_LENGTH] = L"UNKNOWN";


                    hr = pPrfInfo->GetClassIDInfo( classID, 
                                                   NULL,  
                                                   &classToken );
                    
                    if SUCCEEDED( hr )
                    {
                        if ( classToken != mdTypeDefNil )
                        {
                            hr = GetClassName(pMDImport, classToken, className);
                        }
                        _snwprintf( functionName, functionNameLen, L"%s::%s", className, funName );                    
                        functionName[functionNameLen-1] = L'\0';


                        DWORD methodAttr = 0;
                        PCCOR_SIGNATURE sigBlob = NULL;


                        hr = pMDImport->GetMethodProps( (mdMethodDef) token,
                                                        NULL,
                                                        NULL,
                                                        0,
                                                        NULL,
                                                        &methodAttr,
                                                        &sigBlob,
                                                        NULL,
                                                        NULL,
                                                        NULL );
                        if ( SUCCEEDED( hr ) )
                        {
                            ULONG callConv;


                            //
                            // Is the method static ?
                            //
                            (*isStatic) = (BOOL)((methodAttr & mdStatic) != 0);

                            //
                            // Make sure we have a method signature.
                            //
                            char buffer[2 * MAX_LENGTH];
                            
                            
                            sigBlob += CorSigUncompressData( sigBlob, &callConv );
                            if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
                            {
                                static WCHAR* callConvNames[8] = 
                                {   
                                    L"", 
                                    L"unmanaged cdecl ", 
                                    L"unmanaged stdcall ",  
                                    L"unmanaged thiscall ", 
                                    L"unmanaged fastcall ", 
                                    L"vararg ",  
                                    L"<error> "  
                                    L"<error> "  
                                };  
                                buffer[0] = '\0';
                                if ( (callConv & 7) != 0 )
                                    sprintf( buffer, "%s ", callConvNames[callConv & 7]);   
                                
                                //
                                // Grab the argument count
                                //
                                sigBlob += CorSigUncompressData( sigBlob, argCount );

                                //
                                // Get the return type
                                //
                                sigBlob = ParseElementType( pMDImport, sigBlob, buffer );

                                //
                                // if the return typ returned back empty, write void
                                //
                                if ( buffer[0] == '\0' )
                                    sprintf( buffer, "void" );

                                _snwprintf( returnTypeStr, returnTypeStrLen, L"%S",buffer );
                                returnTypeStr[returnTypeStrLen-1] = L'\0';
                                
                                //
                                // Get the parameters
                                //                              
                                for ( ULONG i = 0; 
                                      (SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount))); 
                                      i++ )
                                {
                                    buffer[0] = '\0';

                                    sigBlob = ParseElementType( pMDImport, sigBlob, buffer );                                   
                                    if ( i == 0 ) {
                                        _snwprintf( functionParameters, functionParametersLen, L"%S", buffer );
                                        functionParameters[functionParametersLen-1] = L'\0';
                                    }

                                    else if ( sigBlob != NULL ) {
                                        _snwprintf( functionParameters, functionParametersLen, L"%s+%S", functionParameters, buffer );
                                        functionParameters[functionParametersLen-1] = L'\0';
                                    }
                                    
                                    else
                                        hr = E_FAIL;
                                }                                                                   
                            }
                            else
                            {
                                //
                                // Get the return type
                                //
                                buffer[0] = '\0';
                                sigBlob = ParseElementType( pMDImport, sigBlob, buffer );
                                _snwprintf( returnTypeStr, returnTypeStrLen, L"%s %S",returnTypeStr, buffer );
                                returnTypeStr[returnTypeStrLen-1] = L'\0';
                            }
                        } 
                    } 
                } 

                pMDImport->Release();
            }       
        } 
    }
    //
    // This corresponds to an unmanaged frame
    //
    else
    {
        //
        // Set up return parameters
        //
        hr = S_OK;
        *argCount = 0;
        *isStatic = FALSE;
        returnTypeStr[0] = NULL; 
        functionParameters[0] = NULL;
        wcsncpy( functionName, L"UNMANAGED FRAME", functionNameLen );   
        functionName[functionNameLen-1] = L'\0';
    }

    
    return hr;

} // BASEHELPER::GetFunctionProperties


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
void BASEHELPER::Indent( DWORD indent )
{
    for ( DWORD i = 0; i < indent; i++ )
        LOG_TO_FILE( ("   ") )

} // BASEHELPER::Indent


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
/* public */
DWORD BASEHELPER::GetRegistryKey( char *regKeyName )
{
    DWORD type;
    DWORD size;
    DWORD retValue = -1;
    HKEY userKey = NULL;
    HKEY machineKey = NULL;


    //
    // local machine
    //
    size = 4;
    if ( (RegOpenKeyExA( HKEY_LOCAL_MACHINE, 
                         EE_REGISTRY_ROOT,
                         0, 
                         KEY_READ, 
                         &machineKey ) == ERROR_SUCCESS) &&

          (RegQueryValueExA( machineKey, 
                             regKeyName, 
                             0, 
                             &type, 
                             (LPBYTE)&retValue, 
                             &size ) == ERROR_SUCCESS) &&
          
          (type == REG_DWORD) )
    {                                       
        printf( "Registry LM Variable: %s=%d\n", regKeyName, retValue );
    }
    //
    // current user
    //
    else if ( (RegOpenKeyExA( HKEY_CURRENT_USER, 
                              EE_REGISTRY_ROOT, 
                              0, 
                              KEY_READ, 
                              &userKey ) == ERROR_SUCCESS) &&
             
              (RegQueryValueExA( userKey, 
                                 regKeyName, 
                                 0, 
                                 &type, 
                                 (LPBYTE)&retValue, 
                                 &size ) == ERROR_SUCCESS) &&

              (type == REG_DWORD) )
    {                                       
        printf( "Registry CU Variable: %s=%d\n", regKeyName, retValue );
    }

    RegCloseKey( userKey );                
    RegCloseKey( machineKey );
    
    
    return retValue; 

} // BASEHELPER::GetRegistryKey


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
ULONG BASEHELPER::GetElementType( PCCOR_SIGNATURE pSignature, CorElementType *pType, BOOL bDeepParse )
{
    ULONG index = 0;
    mdToken typeRef;
    ULONG elementType;
    ULONG tempType;
    
    
    // picking apart primitive types is easy;  
    // the ones below require a bit more processing
    index += CorSigUncompressData( &pSignature[index], &elementType );                   
    switch ( elementType )
    {
        // SENTINEL, PINNED and BYREF are not types, just modifiers
        case ELEMENT_TYPE_SENTINEL:
        case ELEMENT_TYPE_BYREF:    
        case ELEMENT_TYPE_PINNED:
                DEBUG_OUT( ("**** PROCESSING SENTINEL/PINNED/BYREF ****") )
                index += GetElementType( &pSignature[index], (CorElementType *)&elementType, bDeepParse );
                break;


        case ELEMENT_TYPE_PTR:
        case ELEMENT_TYPE_SZARRAY:  
                DEBUG_OUT( ("**** PROCESSING PTR/SZARRAY ****") )
                if ( bDeepParse )
                    index += GetElementType( &pSignature[index], (CorElementType *)&elementType );
                else
                    index += GetElementType( &pSignature[index], (CorElementType *)&tempType );

                break;
                        
                        
        case ELEMENT_TYPE_CLASS:
        case ELEMENT_TYPE_VALUETYPE:                                             
                DEBUG_OUT( ("**** PROCESSING CLASS/OBJECT/VALUECLASS ****") )
                index += CorSigUncompressToken( &pSignature[index], &typeRef );
                break;                   
                    

        case ELEMENT_TYPE_CMOD_OPT:
        case ELEMENT_TYPE_CMOD_REQD:                                                            
                DEBUG_OUT( ("**** PROCESSING CMOD_OPT/CMOD_REQD ****") )
                index += CorSigUncompressToken( &pSignature[index], &typeRef ); 
                if ( bDeepParse )
                    index += GetElementType( &pSignature[index], (CorElementType *)&elementType );                                                                                                                                 
                else
                    index += GetElementType( &pSignature[index], (CorElementType *)&tempType );

                break;                                            


        case ELEMENT_TYPE_ARRAY:     
                DEBUG_OUT( ("**** PROCESSING ARRAY ****") )
                if ( bDeepParse )
                    index += ProcessArray( &pSignature[index], (CorElementType *)&elementType );                                                                                                                                   
                else
                    index += ProcessArray( &pSignature[index], (CorElementType *)&tempType );

                break;


        case ELEMENT_TYPE_FNPTR:     
                DEBUG_OUT( ("**** PROCESSING FNPTR ****") )

                // !!! this will throw exception !!!
                index += ProcessMethodDefRef( &pSignature[index], (CorElementType *)&tempType );                                                                                                                                   

                break;                                            
                
    } // switch

    *pType = (CorElementType)elementType;
    

    return index;

} // BASEHELPER::GetElementType


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
ULONG BASEHELPER::ProcessArray( PCCOR_SIGNATURE pSignature, CorElementType *pType )
{
    ULONG index = 0;
    ULONG rank;


    index += GetElementType( &pSignature[index], pType );                                                                                                                                  
    index += CorSigUncompressData( &pSignature[index], &rank );
    if ( rank > 0 )
    {
        UINT i;
        ULONG sizes;
        ULONG lowers;


        index += CorSigUncompressData( &pSignature[index], &sizes );
        for ( i = 0; i < sizes; i++ ) 
        {
            ULONG dimension;


            index += CorSigUncompressData( &pSignature[index], &dimension );
        } // for

        
        index += CorSigUncompressData( &pSignature[index], &lowers );
        for ( i = 0; i < lowers; i++ )
        {
            int lowerBound;


            index += CorSigUncompressSignedInt( &pSignature[index], &lowerBound );
        } // for
    }


    return index;
    
} // BASEHELPER::ProcessArray


/***************************************************************************************
 *  Method:
 *
 *
 *  Purpose:
 *
 *
 *  Parameters: 
 *
 *
 *  Return value:
 *
 *
 *  Notes:
 *
 ***************************************************************************************/
DECLSPEC
/* static public */
ULONG BASEHELPER::ProcessMethodDefRef( PCCOR_SIGNATURE pSignature, CorElementType *pType )
{
    _THROW_EXCEPTION( "**** ELEMENT_TYPE_FNPTR not supported by the framework ****" )

    return 0;
    
} // BASEHELPER::ProcessArray

// End of File
 
