namespace SpaceScience;

public class Class1
{
    private static double PI = 3.14159265358979323846;
    private double GR = 180.0 / PI;      //Из гр. в рад.(/GR)
                                         //Из рад. в гр.(*GR)
    private double mu = 398600.44;    //Гравитационная постоянная
    private double T3 = 86164;        //
                                      //  private double w3 = 0.000072921;  //Угловая скорость вращения Земли, рад/с
    private double R = 6371.110;
    //  double r;                     //Радиус-вектор
    //  double ph;                     //Фокальный параметр
    //  double a;                     //Большая полуось, км
    //  double e;                     //Эксцентриситет
    //  double i;                     //Наклонение, град
    //  double Om;                    //Долгота восходящего узла, град
    //  double w;                     //Аргумент перигея, град
    //  double v;                     //Истинная аномалия, рад
    //  double t;                     //Время рассчёта, с
    //  double u;                     //Аргумент широты
    //  double t0;                    //Время прохождения перигея, с
    //  double dt;                    //Временной шаг
    //  double T;                     //Период обращения ИСЗ
    // int GOD, MES, DEN;            //
    // double S0;                    //Среднее звёздное время, рад
    // double tau;                   //
    // double teg;                   //Гринвичское время пересечения ИСЗ плоскости экватора, с
    // double tem;                   //Московское время, с
    //double dltka;                  //Центральный угол полосы обзора
    //double dltob;                  //Центральный угол до объекта наблюдения
    //double dltpredel;              //Предельный центральный угол КА
    //  double gam1, gam2;                   //Угол обзора с борта ИСЗ
    //  double sig1, sig2;                   //Угол возвышения наблюдаемой точки над горизонтом
    //  double H;                     //Высота ИСЗ над поверхностью Земли

    class grintr
    {
        public double lm;
        public double dl;
    };

    class polobz
    {
        public double dlLev1, lmLev1, dlPrav1, lmPrav1;
        public double dlLev2, lmLev2, dlPrav2, lmPrav2;
    };

    // ANOMALIA
    private double anom(double a, double e, double t, double t0)
    {
        double v;
        double M, E0, E1 = 0, a1;
        double eps = Math.Pow(1.0, -5);
        M = Math.Sqrt(mu / a) / a * (t - t0);
        if (e < 1)
        {
            E0 = M;
            E1 = M + e * Math.Sin(E0);
            while (Math.Abs(E1 - E0) >= eps)
            {
                E0 = E1;
                E1 = M + e * Math.Sin(E0);
            }
            if (Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E1 / 2) >= 0)
            { v = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E1 / 2)); }
            else
            { v = 2 * (Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E1 / 2)) + PI); }
            return v;
        }
        E0 = 0;
        a1 = (M + E1) / e;
        E1 = Math.Log(a1 + Math.Sqrt(a1 * a1 + 1));
        while (Math.Abs(E1 - E0) >= eps)
        {
            E0 = E1;
            a1 = (M + E0) / e;
            E1 = Math.Log(a1 + Math.Sqrt(a1 * a1 + 1));
        }
        v = 2 * Math.Atan(Math.Sqrt((e + 1) / (e - 1)) * Math.Tanh(E1 / 2));
        return v;
    }

    // Рассчёт широты и долготы в ГСО
    grintr gr_tr(double i, double lmgr, double u, double T)
    {
        grintr gr = new();

        if (u > 2 * PI)
        { u = u - 2 * PI; }
        gr.dl = Math.Asin(Math.Sin(i) * Math.Sin(u));

        if (u <= PI / 2 && u >= 0)
        { gr.lm = lmgr + Math.Asin(Math.Tan(gr.dl) / Math.Tan(i)) - (T / T3) * u; }
        if (u > PI / 2 && u <= 3 * PI / 2)
        { gr.lm = lmgr + PI - Math.Asin(Math.Tan(gr.dl) / Math.Tan(i)) - (T / T3) * u; }
        if (u > 3 * PI / 2 && u < 2 * PI)
        { gr.lm = lmgr + 2 * PI + Math.Asin(Math.Tan(gr.dl) / Math.Tan(i)) - (T / T3) * u; }
        return gr;
    }

    private double sin(double value) => Math.Sin(value);
    private double cos(double value) => Math.Cos(value);
    private double asin(double value) => Math.Asin(value);
    private double acos(double value) => Math.Acos(value);
    private double tan(double value) => Math.Tan(value);
    private double floor(double value) => Math.Floor(value);
    private double fabs(double value) => Math.Abs(value);
    private double sqrt(double value) => Math.Sqrt(value);
    private double fmod(double value, double mod) => value % mod;// Math.Fmod(value);

    // Построение полосы обзора
    polobz pol_obz(double a, double e, double i, double lmgr, double w, double u, double T, double z1, double z2)
    {
        polobz obz = new();
        double uLev1, uLev2, uPrav1, uPrav2, iLev1, iLev2, iPrav1, iPrav2;
        var gam1 = PI / 2 - z1;
        var gam2 = PI / 2 - z2;
        var ph = a * (1 - e * e);
        var H = ph / (1 + e * Math.Cos(u - w)) - R;
        var sig1 = Math.Acos(((R + H) * Math.Sin(gam1)) / R);
        var sig2 = Math.Acos(((R + H) * Math.Sin(gam2)) / R);
        double dlpr1 = PI / 2 - sig1 - gam1;
        double dlpr2 = PI / 2 - sig2 - gam2;
        //double b=2*R*(PI/2-gam-sig);
        //double b=atan((tan(gam)*H)/R);
        //u=w+v;
        uLev1 = Math.Acos(Math.Cos(dlpr1) * Math.Cos(u));
        iLev1 = i + Math.Atan2(Math.Tan(dlpr1), Math.Sin(u));
        uPrav1 = Math.Acos(Math.Cos(dlpr1) * Math.Cos(u));
        iPrav1 = i - Math.Atan2(Math.Tan(dlpr1), Math.Sin(u));

        uLev2 = Math.Acos(Math.Cos(dlpr2) * Math.Cos(u));
        iLev2 = i + Math.Atan2(Math.Tan(dlpr2), Math.Sin(u));
        uPrav2 = Math.Acos(Math.Cos(dlpr2) * Math.Cos(u));
        iPrav2 = i - Math.Atan2(Math.Tan(dlpr2), sin(u));

        obz.dlLev1 = asin(sin(uLev1) * sin(iLev1));
        obz.dlPrav1 = asin(sin(uPrav1) * sin(iPrav1));
        obz.dlLev2 = asin(sin(uLev2) * sin(iLev2));
        obz.dlPrav2 = asin(sin(uPrav2) * sin(iPrav2));

        if (uLev1 <= PI / 2 && uLev1 >= 0)
        { obz.lmLev1 = lmgr + asin(Math.Tan(obz.dlLev1) / Math.Tan(iLev1)) - (T / T3) * u; }
        if (uLev1 > PI / 2 && uLev1 <= 3 * PI / 2)
        { obz.lmLev1 = lmgr + PI - asin(Math.Tan(obz.dlLev1) / Math.Tan(iLev1)) - (T / T3) * u; }
        if (uLev1 > 3 * PI / 2 && uLev1 < 2 * PI)
        { obz.lmLev1 = lmgr + 2 * PI + asin(Math.Tan(obz.dlLev1) / Math.Tan(iLev1)) - (T / T3) * u; }
        if (uPrav1 <= PI / 2 && uPrav1 >= 0)
        { obz.lmPrav1 = lmgr + asin(Math.Tan(obz.dlPrav1) / Math.Tan(iPrav1)) - (T / T3) * u; }
        if (uPrav1 > PI / 2 && uPrav1 <= 3 * PI / 2)
        { obz.lmPrav1 = lmgr + PI - asin(Math.Tan(obz.dlPrav1) / Math.Tan(iPrav1)) - (T / T3) * u; }
        if (uPrav1 > 3 * PI / 2 && uPrav1 < 2 * PI)
        { obz.lmPrav1 = lmgr + 2 * PI + asin(Math.Tan(obz.dlPrav1) / Math.Tan(iPrav1)) - (T / T3) * u; }

        if (uLev2 <= PI / 2 && uLev2 >= 0)
        { obz.lmLev2 = lmgr + asin(Math.Tan(obz.dlLev2) / Math.Tan(iLev2)) - (T / T3) * u; }
        if (uLev2 > PI / 2 && uLev2 <= 3 * PI / 2)
        { obz.lmLev2 = lmgr + PI - asin(tan(obz.dlLev2) / tan(iLev2)) - (T / T3) * u; }
        if (uLev2 > 3 * PI / 2 && uLev2 < 2 * PI)
        { obz.lmLev2 = lmgr + 2 * PI + asin(tan(obz.dlLev2) / tan(iLev2)) - (T / T3) * u; }
        if (uPrav2 <= PI / 2 && uPrav2 >= 0)
        { obz.lmPrav2 = lmgr + asin(tan(obz.dlPrav2) / tan(iPrav2)) - (T / T3) * u; }
        if (uPrav2 > PI / 2 && uPrav2 <= 3 * PI / 2)
        { obz.lmPrav2 = lmgr + PI - asin(tan(obz.dlPrav2) / tan(iPrav2)) - (T / T3) * u; }
        if (uPrav2 > 3 * PI / 2 && uPrav2 < 2 * PI)
        { obz.lmPrav2 = lmgr + 2 * PI + asin(tan(obz.dlPrav2) / tan(iPrav2)) - (T / T3) * u; }
        return obz;
    }

    //!!!!!!!!!!!!!!!!!!!!!!!(1 этап)!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //Рассчёт времени и номера витка объекта,попавшего в полосу обзора(левую или правую),
    //из katalog, при котором объект будет наблюдаем
    public IList<(double obj_nomer, double B, double L, int nomer_vitka, int lev_polosa, int prav_polosa, double t_vid)>
            Button2Click(IList<(double b, double l, double obj_n)> targets)
    {
        double t0 = 0.0;
        double u0 = 0;

        double a = 6948.0;//a=6948, H=577
        double e = 0.0;//e=0
        double i = 97.65;//i=97.65
        double w = 0.0;//w=0
        double lmgr = 0.0;//lmgr=0
        //double tau = 180.0;//tau=180
        i = i / GR;
        w = w / GR;
        lmgr = lmgr / GR;
        //tau = tau / GR;
        //    double DEN = 22;//DEN=22
        //    double MES = 6;//MES=6
        //    double GOD = 2010;//GOD=2010
        double nodeCount = 15;//nvit=15
        double z1 = 70;//z1=70  gam1=20
        double z2 = 35;//z2=35  gam2=55
        z1 = z1 / GR;
        z2 = z2 / GR;

        var T = 2 * PI * sqrt(a * a * a / mu);
        double n11 = floor(T3 / T);
        double m11 = T3 / T - n11;

        var gam1 = PI / 2 - z1;
        var gam2 = PI / 2 - z2;
        var ph = a * (1 - e * e);
        var H = ph / (1 + e * cos(u0 - w)) - R;

        double dlt1 = (PI / 2 - gam1 - acos((R + H) * sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double dlt2 = (PI / 2 - gam2 - acos((R + H) * sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора
        double dltpredel = acos(R / (R + H)) * GR;//2 центр угол (вроде не нужен, но хз)


        var list = new List<(double obj_nomer, double B, double L, int nomer_vitka, int lev_polosa, int prav_polosa, double t_vid)>();


        foreach (var (B, Lres, OBJ_N) in targets)
        {
            double L = Lres;

            double dltka1 = dlt1;
            double dltka2 = dlt2;
            while (L > PI) { L -= 2 * PI; }
            while (L < -PI) { L += 2 * PI; }
            int counter = 0;      //счётчик для совершения суточного сдвига
            int j = 0;            //Коэффициент при суточном сдвиге
            int nomervitka = 0;   //Количество витков
            double l, m;
            double l1LEV = 0, m1LEV = 0, l1PR = 0, m1PR = 0;
            double lmlev1, dlprav1, dllev1, lmprav1;
            double t_vid = 0;
            // double Tk_vid, Tn_vid;
            for (int s = 0; s < nodeCount; s++)
            {
                double rMin = 10000;
                counter++;
                nomervitka++;

                int COUNTER = 0;

                if (counter > n11) { j++; counter = 0; }

                for (double t = 0; t <= T; t = t + 1)
                {
                    var v = anom(a, e, t, t0);
                    var u = v + w;
                    grintr trassa = gr_tr(i, lmgr, u, T);
                    polobz polosa = pol_obz(a, e, i, lmgr, w, u, T, z1, z2);
                    l = trassa.dl;
                    m = trassa.lm - (T / T3) * 2 * PI * s - (1 - m11) * (T / T3) * 2 * PI * j;

                    while (m > PI) { m -= 2 * PI; }

                    while (m < -PI) { m += 2 * PI; }

                    dllev1 = polosa.dlLev1;
                    lmlev1 = polosa.lmLev1 - (T / T3) * 2 * PI * s - (1 - m11) * (T / T3) * 2 * PI * j;
                    dlprav1 = polosa.dlPrav1;
                    lmprav1 = polosa.lmPrav1 - (T / T3) * 2 * PI * s - (1 - m11) * (T / T3) * 2 * PI * j;

                    while (lmlev1 > PI) { lmlev1 -= 2 * PI; }
                    while (lmprav1 > PI) { lmprav1 -= 2 * PI; }
                    while (lmlev1 < -PI) { lmlev1 += 2 * PI; }
                    while (lmprav1 < -PI) { lmprav1 += 2 * PI; }

                    //(X, Y, Z) - Координаты подспутниковой точки
                    double X = R * cos(m);
                    double Y = R * sin(m);
                    double Z = R * sin(l) / sqrt(1 - sin(l) * sin(l));
                    //(x, y, z) - Координаты объекта наблюдения
                    double x = R * cos(L);
                    double y = R * sin(L);
                    double z = R * sin(B) / sqrt(1 - sin(B) * sin(B));

                    double dltob = acos((x * X + y * Y + z * Z) / (sqrt(x * x + y * y + z * z) * sqrt(X * X + Y * Y + Z * Z))) * GR;//2
                    if (dltob < dltka1) { COUNTER = -1000; }
                    if (dltob <= dltka2 && dltob >= dltka1 && dltob <= dltpredel && dltob >= (-dltpredel))
                    {
                        COUNTER++;
                        if (dltob < rMin)
                        {
                            dltob = rMin;
                            t_vid = t;
                            l1LEV = dllev1;
                            m1LEV = lmlev1;
                            l1PR = dlprav1;
                            m1PR = lmprav1;
                        }
                        //if(COUNTER==1){Tn_vid=t;}
                        //Tk_vid=t;
                    }
                }//for (t=0; t<=T; t=t+15)

                if (COUNTER > 0)
                {
                    //double Tvidim=Tk_vid-Tn_vid;
                    //Table2->FieldByName("t_vidimosti")->AsFloat=Tvidim;
                    double rLev, rPrav;
                    double xob = R * cos(L);
                    double yob = R * sin(L);
                    double zob = R * sin(B) / sqrt(1 - sin(B) * sin(B));
                    double xLev = R * cos(m1LEV);
                    double yLev = R * sin(m1LEV);
                    double zLev = R * sin(l1LEV) / sqrt(1 - sin(l1LEV) * sin(l1LEV));
                    double xPrav = R * cos(m1PR);
                    double yPrav = R * sin(m1PR);
                    double zPrav = R * sin(l1PR) / sqrt(1 - sin(l1PR) * sin(l1PR));
                    rLev = acos((xob * xLev + yob * yLev + zob * zLev) / (sqrt(xob * xob + yob * yob + zob * zob) * sqrt(xLev * xLev + yLev * yLev + zLev * zLev))) * GR;//2
                    rPrav = acos((xob * xPrav + yob * yPrav + zob * zPrav) / (sqrt(xob * xob + yob * yob + zob * zob) * sqrt(xPrav * xPrav + yPrav * yPrav + zPrav * zPrav))) * GR;//2
                    int lev_polosa;
                    int prav_polosa;
                    if (rLev < rPrav)
                    {
                        lev_polosa = 1;
                        prav_polosa = 0;
                    }
                    else
                    {
                        prav_polosa = 1;
                        lev_polosa = 0;
                    }

                    list.Add((OBJ_N, B, L, nomervitka, lev_polosa, prav_polosa, t_vid));

                }
            }//for (int s=0; s<n; s++)

        }

        return list;
    }

}