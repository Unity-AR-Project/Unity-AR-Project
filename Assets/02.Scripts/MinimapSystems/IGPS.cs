namespace ARP.MinimapSystems
{
    //주석
    // 1. /**/ : 프로그래밍과 크게 상관없는 내용
    // 2. <summary> : 사용자 정의 자료형 위, 기능위에 사용

    /// <summary>
    /// 기기의 위도, 경도, 고도 정보
    /// </summary>
    public interface IGPS
    {
        /// <summary>
        /// 위도
        /// </summary>
        float latitude { get; }

        /// <summary>
        /// 경도
        /// </summary>
        float longitude { get; }

        /// <summary>
        /// 고도
        /// </summary>
        float altitude { get; }
    }
}