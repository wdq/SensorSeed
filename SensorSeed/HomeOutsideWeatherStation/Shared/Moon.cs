using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeOutsideWeatherStation.Shared
{
    class Moon
    {
        /*  Astronomical constants  */
        public const double epoch = 2444238.5; /* 1980 January 0.0 */
        public const double J1970 = 2440587.5; /* VAX clock Epoch 1970 Jan 1 (0h UT) */

        /*  Constants defining the Sun's apparent orbit  */
        public const double elonge = 278.833540; /* Ecliptic longitude of the Sun at epoch 1980.0 */
        public const double elongp = 282.596403; /* Ecliptic longitude of the Sun at perigee */
        public const double eccent = 0.016718; /* Eccentricity of Earth's orbit */
        public const double sunsmax = 1.495985e8; /* Semi-major axis of Earth's orbit, km */
        public const double sunangsiz = 0.533128; /* Sun's angular size, degrees, at semi-major axis distance */

        /*  Elements of the Moon's orbit, epoch 1980.0  */
        public const double mmlong = 64.975464;      /* Moon's mean lonigitude at the epoch */
        public const double mmlongp = 349.383063;      /* Mean longitude of the perigee at the epoch */
        public const double mlnode = 151.950429;       /* Mean longitude of the node at the epoch */
        public const double minc = 5.145396;       /* Inclination of the Moon's orbit */
        public const double mecc = 0.054900;       /* Eccentricity of the Moon's orbit */
        public const double mangsiz = 0.5181;         /* Moon's angular size at distance a from Earth */
        public const double msmax = 384401.0;       /* Semi-major axis of Moon's orbit in km */
        public const double mparallax = 0.9507;    /* Parallax at distance a from Earth */
        public const double synmonth = 29.53058868;    /* Synodic month (new Moon to new Moon) */
        public const double lunatbase = 2423436.0;      /* Base date for E. W. Brown's numbered series of lunations (1923 January 16) */

        /*  Properties of the Earth  */
        public const double earthrad = 6378.16;    /* Radius of Earth in kilometres */
        public const double PI = 3.14159265358979323846;  /* Assume not near black hole nor in Tennessee */

        /*  Handy mathematical functions  */
        public double sgn(double x) /* Extract sign */
        {
            return (((x) < 0) ? -1 : ((x) > 0 ? 1 : 0));
        }

        public double abs(double x) /* Absolute val */
        {
            return ((x) < 0 ? (-(x)) : (x));
        }

        public double fixangle(double a) /* Fix angle	  */
        {
            return ((a) - 360.0*(Math.Floor((a)/360.0)));
        }

        public double torad(double d) /* Deg->Rad	  */
        {
            return ((d)*(PI/180.0));
        }

        public double todeg(double d) /* Rad->Deg	  */
        {
            return ((d)*(180.0/PI));
        }

        public double dsin(double x) /* Sin from deg */
        {
            return (Math.Sin(torad((x))));
        }

        public double dcos(double x) /* Cos from deg */
        {
            return (Math.Cos(torad((x))));
        }




    }
}
